using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;

namespace Vault.Shared.Authentication.Pocket
{
    public class PocketRedirectToAuthorizationEndpointContext : BasePocketContext
    {
        public PocketRedirectToAuthorizationEndpointContext(
            HttpContext context,
            PocketOptions options,
            AuthenticationProperties properties,
            string redirectUri)
            : base(context, options)
        {
            RedirectUri = redirectUri;
            Properties = properties;
        }

        /// <summary>
        /// Gets the URI used for the redirect operation.
        /// </summary>
        public string RedirectUri { get; private set; }

        /// <summary>
        /// Gets the authentication properties of the challenge.
        /// </summary>
        public AuthenticationProperties Properties { get; private set; }
    }
}