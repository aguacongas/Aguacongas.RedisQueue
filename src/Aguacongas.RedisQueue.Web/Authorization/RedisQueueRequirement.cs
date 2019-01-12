using Microsoft.AspNetCore.Authorization;

namespace Aguacongas.RedisQueue
{
    /// <summary>
    /// RedisQueue requirement used by RedisQueuePolicy
    /// </summary>
    /// <seealso cref="IAuthorizationRequirement" />
    public class RedisQueueRequirement : IAuthorizationRequirement
    {
    }
}