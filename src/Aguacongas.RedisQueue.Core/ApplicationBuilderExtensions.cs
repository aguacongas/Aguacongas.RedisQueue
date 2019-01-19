using Aguacongas.RedisQueue;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRedisQueue(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<IManageQueues>();
                manager.RebuildIndexesAndSubscribe();
            }
            return builder;
        }
    }
}
