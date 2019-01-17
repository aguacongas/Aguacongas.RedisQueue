using Aguacongas.RedisQueue.Authentication;
using Microsoft.AspNetCore.Authentication;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class AuthenticationBuilderExtensions
    {
        /// <summary>
        /// Adds the no authentication.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddNoAuthentication(this AuthenticationBuilder builder)
        {
            return builder.AddNoAuthentication(NoAuthenticationDefault.AuthenticationScheme);
        }

        /// <summary>
        /// Adds the no authentication.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scheme">The scheme.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddNoAuthentication(this AuthenticationBuilder builder, string scheme)
        {
            return builder.AddNoAuthentication(scheme, configureOptions: null);
        }

        /// <summary>
        /// Adds the no authentication.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddNoAuthentication(this AuthenticationBuilder builder, Action<NoAuthenticationOptions> configureOptions)
        {
            return builder.AddNoAuthentication(NoAuthenticationDefault.AuthenticationScheme, configureOptions);
        }

        /// <summary>
        /// Adds the no authentication.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scheme">The scheme.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddNoAuthentication(this AuthenticationBuilder builder, string scheme, Action<NoAuthenticationOptions> configureOptions)
        {
            return builder.AddNoAuthentication(scheme, scheme, configureOptions);
        }

        /// <summary>
        /// Adds the no authentication.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scheme">The scheme.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddNoAuthentication(this AuthenticationBuilder builder, string scheme, string displayName, Action<NoAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<NoAuthenticationOptions, NoAuthenticationHandler>(scheme, displayName, configureOptions);
        }
    }
}
