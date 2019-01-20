using Aguacongas.RedisQueue;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace server.Authorization
{
    /// <summary>
    /// Can use queues authorization handler
    /// </summary>
    /// <seealso cref="AuthorizationHandler{RedisQueueRequirement}" />
    public class CanUseQueuesHandler : AuthorizationHandler<RedisQueueRequirement>
    {
        /// <summary>
        /// Makes a decision if authorization is allowed based on a specific requirement.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The requirement to evaluate.</param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RedisQueueRequirement requirement)
        {
            // TODO: implement your logic to authorize your users to access to queues
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
