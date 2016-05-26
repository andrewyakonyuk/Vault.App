using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace Vault.Shared.Authentication.Pocket
{
    public class PocketCreatingTicketContext : BasePocketContext
    {
        public PocketCreatingTicketContext(
            HttpContext context,
            PocketOptions options,
            string userId,
            string screenName,
            string accessToken,
            JObject user)
            : base(context, options)
        {
            UserId = userId;
            ScreenName = screenName;
            AccessToken = accessToken;
            User = user ?? new JObject();
        }

        /// <summary>
        /// Gets the Pocket user ID
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Gets the Pocket screen name
        /// </summary>
        public string ScreenName { get; }

        /// <summary>
        /// Gets the Pocket access token
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// Gets the JSON-serialized user or an empty
        /// <see cref="JObject"/> if it is not available.
        /// </summary>
        public JObject User { get; }

        /// <summary>
        /// Gets the <see cref="ClaimsPrincipal"/> representing the user
        /// </summary>
        public ClaimsPrincipal Principal { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }
    }
}