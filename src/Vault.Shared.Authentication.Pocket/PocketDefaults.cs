namespace Vault.Shared.Authentication.Pocket
{
    public class PocketDefaults
    {
        public const string AuthenticationScheme = "Pocket";

        public const string AuthorizationEndpoint = "https://getpocket.com/auth/authorize";

        public const string AccessTokenEndpoint = "https://getpocket.com/v3/oauth/authorize";

        public const string RequestTokenEndpoint = "https://getpocket.com/v3/oauth/request";
    }
}