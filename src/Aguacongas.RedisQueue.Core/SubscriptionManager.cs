using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Aguacongas.RedisQueue
{
    public class SubscriptionManager : IManageSubscription
    {
        private static readonly Regex _isStatic = new Regex("^(http|https)://");

        private readonly ConcurrentDictionary<string, ManagerBase> _managers = new ConcurrentDictionary<string, ManagerBase>();
        private readonly ISubscriber _subscriber;
        private readonly IStore _store;
        private readonly HttpClient _httpClient;

        public async Task Handle(RedisChannel channel, RedisValue value)
        {
            var manager = _managers.GetOrAdd(channel, c =>
            {
                if (IsLocal(c))
                {
                    return new LocalManager();
                }
                return new RemoteManager(_httpClient, _store, _subscriber);
            });
            manager = await manager.Handle(value).ConfigureAwait(false);
            _managers.AddOrUpdate(channel, manager, (c, m) => manager);
        }

        private bool IsLocal(string address)
        {
            return _isStatic.IsMatch(address);
        }

        abstract class ManagerBase
        {
            public string Address { get; set; }
            public abstract Task<ManagerBase> Handle(string value);
        }

        class LocalManager : ManagerBase
        {
            public override Task<ManagerBase> Handle(string value)
            {
                return Task.FromResult((ManagerBase)this);
            }
        }

        class RemoteManager: ManagerBase
        {
            private readonly IStore _store;
            private readonly ISubscriber _subscriber;
            private readonly HttpClient _httpClient;

            public RemoteManager(HttpClient httpClient, IStore store, ISubscriber subscriber)
            {
                _httpClient = httpClient;
                _store = store;
                _subscriber = subscriber;
            }

            public override async Task<ManagerBase> Handle(string value)
            {
                string messageId = value;

                var message = await _store.Peek(Address).ConfigureAwait(false);
                if (message == null)
                {
                    return this;
                }

                var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(Address, content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        return new RemoteFailManager(this, _subscriber, _store, _httpClient);
                    }
                }

                await _store.Remove(message.Id, Address).ConfigureAwait(false);
                return this;
            }
        }

        class RemoteFailManager: ManagerBase
        {
            private readonly RemoteManager _parent;
            private readonly ISubscriber _subscriber;
            private readonly IStore _store;
            private readonly HttpClient _httpClient;
            private readonly Timer _timer;

            private ManagerBase _current;

            public RemoteFailManager(RemoteManager parent, ISubscriber subscriber, IStore store, HttpClient httpClient)
            {
                _parent = parent;
                _subscriber = subscriber;
                _store = store;
                _httpClient = httpClient;

                _timer = StartPooling();
                _current = this;
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
                        _current = _parent;
                        // publish a queue event to reset the queue manager
                        await _subscriber.PublishAsync(Address, Guid.Empty.ToString());
                        // publish an event for each message waiting in the queue
                        foreach (var id in await _store.GetKeys(Address))
                        {
                            await _subscriber.PublishAsync(Address, id.ToString());
                        }

                        _timer.Dispose();
                    }
                }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            }
        }
    }
}
