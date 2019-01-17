using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue.Authentication
{
    /// <summary>
    /// No authentication handler
    /// </summary>
    /// <seealso cref="AuthenticationHandler{NoAuthenticationOptions}" />
    public class NoAuthenticationHandler : AuthenticationHandler<NoAuthenticationOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="encoder">The encoder.</param>
        /// <param name="clock">The clock.</param>
        public NoAuthenticationHandler(IOptionsMonitor<NoAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// Handles the authenticate asynchronous.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new NoAuthenticationIdentity()), Scheme.Name)));
        }

        /// <summary>
        /// Override this method to deal with 401 challenge concerns, if an authentication scheme in question
        /// deals an authentication interaction as part of it's request flow. (like adding a response header, or
        /// changing the 401 result to 302 of a login page or external sign-in location.)
        /// </summary>
        /// <param name="properties"></param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method to handle Forbid.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }
    }
}
