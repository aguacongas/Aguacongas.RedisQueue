using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace Aguacongas.RedisQueue
{
    /// <summary>
    /// Services collection extentions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the redis queue to DI.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public static IServiceCollection AddRedisQueue(this IServiceCollection services, string configuration, int? database = null)
        {
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var redisLogger = CreateLogger(provider);

                return ConnectionMultiplexer.Connect(configuration, redisLogger);
            });

            return services
                .AddRedisQueue(provider =>
                {
                    var multiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
                    return multiplexer.GetDatabase(database ?? -1);
                });
        }

        /// <summary>
        /// Adds the redis queue to DI.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="getDatabase">The get database.</param>
        /// <returns></returns>
        public static IServiceCollection AddRedisQueue(this IServiceCollection services, Func<IServiceProvider, IDatabase> getDatabase)
        {
            services
                .AddLogging()
                .AddSingleton(provider =>
                {
                    var db = getDatabase(provider);
                    return db.Multiplexer.GetSubscriber();
                })
                .AddSingleton<IManageSubscription, SubscriptionManager>()
                .AddTransient<IStore>(provider =>
                {
                    var db = getDatabase(provider);
                    var serializer = provider.GetRequiredService<ISerialize>();
                    return new Store(db, serializer);
                })
                .AddTransient<IManageQueues, QueuesManager>()
                .AddTransient<ISerialize, MessageSerializer>();

            return services;
        }

        private static RedisLogger CreateLogger(IServiceProvider provider)
        {
            var logger = provider.GetService<ILogger<RedisLogger>>();
            var redisLogger = logger != null ? new RedisLogger(logger) : null;
            return redisLogger;
        }
    }
}
