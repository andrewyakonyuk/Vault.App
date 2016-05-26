using Microsoft.AspNet.Authentication;
using System.Threading.Tasks;

namespace Vault.Shared.Authentication.Pocket
{
    public class PocketEvents : RemoteAuthenticationEvents, IPocketEvents
    {
        public Task RedirectToAuthorizationEndpoint(PocketRedirectToAuthorizationEndpointContext context)
        {
            context.Response.Redirect(context.RedirectUri);
            return Task.FromResult(0);
        }
    }
}