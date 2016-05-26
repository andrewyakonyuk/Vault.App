using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Http;
using System;

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
            SaveTokensAsClaims = true;
        }

        public ISecureDataFormat<RequestToken> StateDataFormat { get; set; }

        public new IPocketEvents Events
        {
            get { return base.Events as IPocketEvents; }
            set { base.Events = value; }
        }

        public bool SaveTokensAsClaims { get; set; }
    }
}