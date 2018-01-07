using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Vault.Shared.Authentication.Pocket
{
    public static class TwitterExtensions
    {
        public static AuthenticationBuilder AddTwitter(this AuthenticationBuilder builder)
            => builder.AddTwitter(PocketDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddTwitter(this AuthenticationBuilder builder, Action<PocketOptions> configureOptions)
            => builder.AddTwitter(PocketDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddTwitter(this AuthenticationBuilder builder, string authenticationScheme, Action<PocketOptions> configureOptions)
            => builder.AddTwitter(authenticationScheme, PocketDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddTwitter(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<PocketOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<PocketOptions>, PocketPostConfigureOptions>());
            return builder.AddRemoteScheme<PocketOptions, PocketAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
