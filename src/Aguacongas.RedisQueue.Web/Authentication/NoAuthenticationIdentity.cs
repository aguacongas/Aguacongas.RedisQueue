using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Aguacongas.RedisQueue.Authentication
{
    /// <summary>
    /// No authentication identity
    /// </summary>
    /// <seealso cref="System.Security.Claims.ClaimsIdentity" />
    public class NoAuthenticationIdentity : ClaimsIdentity
    {
        private readonly IEnumerable<Claim> _claimCollection = new List<Claim>
        {
            new Claim("Name", "Unknow")
        };
        /// <summary>
        /// Gets a value that indicates whether the identity has been authenticated.
        /// </summary>
        /// <remarks>Always return true</remarks>
        public override bool IsAuthenticated => true;

        /// <summary>
        /// Gets the name of this claims identity.
        /// </summary>
        /// <remarks>Always return null</remarks>
        public override string Name => null;
    }
}
