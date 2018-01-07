using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace Vault.Shared.Authentication.Pocket
{
    public class PocketRedirectToAuthorizationEndpointContext : BaseContext<PocketOptions>
    {
        public PocketRedirectToAuthorizationEndpointContext(
            HttpContext context,
            AuthenticationScheme scheme,
            PocketOptions options,
            AuthenticationProperties properties,
            string redirectUri)
            : base(context, scheme, options)
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