using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Vault.Shared.Authentication.Pocket
{
    public class PocketOptions : RemoteAuthenticationOptions
    {
        public string ConsumerKey { get; set; }

        public PocketOptions()
        {
            AuthenticationScheme = PocketDefaults.AuthenticationScheme;
            DisplayName = AuthenticationScheme;
            CallbackPath = new PathString("/signin-pocket");
            BackchannelTimeout = TimeSpan.FromSeconds(60);
            SaveTokens = true;
        }

        public ISecureDataFormat<RequestToken> StateDataFormat { get; set; }

        public new IPocketEvents Events
        {
            get { return base.Events as IPocketEvents; }
            set { base.Events = value; }
        }
    }
}