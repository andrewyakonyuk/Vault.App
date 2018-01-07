using Microsoft.AspNetCore.Authentication;

namespace Vault.Shared.Authentication.Pocket
{
    public class RequestToken
    {
        public string Token { get; set; }

        public AuthenticationProperties Properties { get; set; }

        public bool CallbackConfirmed { get; set; }
    }
}