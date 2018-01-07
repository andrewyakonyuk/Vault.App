using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace Vault.Shared.Authentication.Pocket
{
    public interface IPocketEvents
    {
        //Task CreatingTicket(TwitterCreatingTicketContext context);

        Task RedirectToAuthorizationEndpoint(PocketRedirectToAuthorizationEndpointContext context);
    }
}