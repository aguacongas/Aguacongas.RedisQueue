using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    /// <summary>
    /// Manage redis subscription
    /// </summary>
    /// <seealso cref="Aguacongas.RedisQueue.IManageSubscription" />
    public class SubscriptionManager : IManageSubscription
    {
        /// <summary>
        /// Gets the is remote regex.
        /// </summary>
        /// <value>
        /// The is remote regex.
        /// </value>
        public static Regex IsRemote { get; } = new Regex("^(http|https):/");

        private readonly ConcurrentDictionary<string, ManagerBase> _managers = new ConcurrentDictionary<string, ManagerBase>();
        private readonly ISubscriber _subscriber;
        private readonly IStore _store;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SubscriptionManager> _logger;

        /// <summary>
        /// Gets the queues names.
        /// </summary>
        /// <value>
        /// The queues.
        /// </value>
        public IEnumerable<string> Queues => _managers.Keys;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionManager"/> class.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="store">The store.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="logger">The logger.</param>
        public SubscriptionManager(ISubscriber subscriber, IStore store, HttpClient httpClient, ILogger<SubscriptionManager> logger)
        {
            _subscriber = subscriber;
            _store = store;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Publishes a message has been queued.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Task Publish(string address, Guid id)
        {
            var manger = GetOrAddManager(address);
            return manger.PublishAsync(id.ToString());
        }

        private ManagerBase GetOrAddManager(string address)
        {
            return _managers.GetOrAdd(address, c =>
            {
                if (IsLocal(c))
                {
                    return new LocalManager(address, _subscriber);
                }
                return new RemoteManager(address, _httpClient, _store, _subscriber, _logger);
            });
        }

        private bool IsLocal(string address)
        {
            return !IsRemote.IsMatch(address);
        }

        abstract class ManagerBase
        {
            public string Address { get; set; }
            public abstract Task<ManagerBase> Handle(string value);

            public abstract Task PublishAsync(string id);
        }

        class LocalManager : ManagerBase
        {
            private readonly ISubscriber _subscriber;

            public LocalManager(string address, ISubscriber subscriber)
            {
                Address = address;
                _subscriber = subscriber;
            }

            public override Task PublishAsync(string id)
            {
                return _subscriber.PublishAsync(Address, id);
            }
            public override Task<ManagerBase> Handle(string value)
            {
                return Task.FromResult((ManagerBase)this);
            }
        }

        class RemoteManager: ManagerBase
        {
            private readonly IStore _store;
            private readonly ISubscriber _subscriber;
            private readonly ILogger<SubscriptionManager> _logger;
            private readonly HttpClient _httpClient;

            public RemoteManager(string address, HttpClient httpClient, IStore store, ISubscriber subscriber, ILogger<SubscriptionManager> logger)
            {
                _httpClient = httpClient;
                _store = store;
                _subscriber = subscriber;
                _logger = logger;

                Address = address;

                _subscriber.Subscribe(address, async (c, v) =>
                {
                    await Handle(v).ConfigureAwait(false);
                });
            }

            public override Task PublishAsync(string id)
            {
                return _subscriber.PublishAsync(Address, id);
            }

            public override async Task<ManagerBase> Handle(string value)
            {               
                Message message;
                while((message = await _store.PopIndexAsync(Address).ConfigureAwait(false)) != null)
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Put, Address))
                    {
                        if (!string.IsNullOrWhiteSpace(message.InitiatorToken))
                        {
                            var tokenInfos = message.InitiatorToken.Split(new char[' '], 2);
                            request.Headers.Authorization = new AuthenticationHeaderValue(tokenInfos[0], tokenInfos[1]);
                        }

                        var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                        request.Content = content;

                        try
                        {
                            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                            if (!response.IsSuccessStatusCode)
                            {
                                if (response.StatusCode != HttpStatusCode.Unauthorized || response.StatusCode != HttpStatusCode.Forbidden)
                                {
                                    return await Requeue(message);
                                }
                                else
                                {
                                    _logger.LogError("Send message {MessageId} to {Address} {StatusCode}", message.Id, Address, response.StatusCode);
                                }
                            }

                            await _store.RemoveDataAsync(message).ConfigureAwait(false);
                        }
                        catch
                        {
                            return await Requeue(message);
                        }
                    }

                }
                return this;
            }

            private async Task<ManagerBase> Requeue(Message message)
            {
                _logger.LogWarning("Connexion to {Address} lost", Address);

                await _store.RepushIndexAsync(message).ConfigureAwait(false);
                return new RemoteFailManager(this, _subscriber, _store, _httpClient, _logger)
                {
                    Address = Address
                };
            }
        }

        class RemoteFailManager: ManagerBase
        {
            private readonly RemoteManager _parent;
            private readonly ISubscriber _subscriber;
            private readonly IStore _store;
            private readonly HttpClient _httpClient;
            private readonly ILogger<SubscriptionManager> _logger;
            private readonly Timer _timer;

            private ManagerBase _current;

            public RemoteFailManager(RemoteManager parent, ISubscriber subscriber, IStore store, HttpClient httpClient, ILogger<SubscriptionManager> logger)
            {
                _parent = parent;
                _subscriber = subscriber;
                _store = store;
                _httpClient = httpClient;
                _logger = logger;

                _timer = StartPooling();
                _current = this;
            }

            public override Task PublishAsync(string id)
            {
                return Task.CompletedTask;
            }
            public override Task<ManagerBase> Handle(string value)
            {
                return Task.FromResult(_current);
            }

            private Timer StartPooling()
            {
                return new Timer(async s =>
                {
                    var response = await _httpClient.GetAsync(Address);
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Connexion to {Address} established", Address);

                        _current = _parent;
                        // publish a queue event to reset the queue manager
                        var message = await _store.PeekAsync(Address);
                        if (message != null)
                        {
                            await _subscriber.PublishAsync(Address, message.Id.ToString());
                        }

                        _timer.Dispose();
                    }
                }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            }
        }
    }
}
