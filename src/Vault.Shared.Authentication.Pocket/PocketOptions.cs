using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Vault.Shared.Authentication.Pocket
{
    public class PocketOptions : RemoteAuthenticationOptions
    {
        private const string DefaultStateCookieName = "__PocketState";

        public string ConsumerKey { get; set; }

        private CookieBuilder _stateCookieBuilder;

        public PocketOptions()
        {
            CallbackPath = new PathString("/signin-pocket");
            BackchannelTimeout = TimeSpan.FromSeconds(60);
            Events = new PocketEvents();
            SaveTokens = true;

            _stateCookieBuilder = new PocketCookieBuilder(this)
            {
                Name = DefaultStateCookieName,
                SecurePolicy = CookieSecurePolicy.SameAsRequest,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            };
        }

        public ISecureDataFormat<RequestToken> StateDataFormat { get; set; }

        public new PocketEvents Events
        {
            get => (PocketEvents)base.Events;
            set => base.Events = value;
        }

        public CookieBuilder StateCookie
        {
            get => _stateCookieBuilder;
            set => _stateCookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override void Validate()
        {
            base.Validate();
            if (string.IsNullOrEmpty(ConsumerKey))
            {
                throw new ArgumentException("Consumer key must be provided", nameof(ConsumerKey));
            }
        }

        private class PocketCookieBuilder : CookieBuilder
        {
            private readonly PocketOptions _pocketOptions;

            public PocketCookieBuilder(PocketOptions twitterOptions)
            {
                _pocketOptions = twitterOptions;
            }

            public override CookieOptions Build(HttpContext context, DateTimeOffset expiresFrom)
            {
                var options = base.Build(context, expiresFrom);
                if (!Expiration.HasValue)
                {
                    options.Expires = expiresFrom.Add(_pocketOptions.RemoteAuthenticationTimeout);
                }
                return options;
            }
        }
    }
}