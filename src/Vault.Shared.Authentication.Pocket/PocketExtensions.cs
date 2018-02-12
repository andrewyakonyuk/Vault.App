using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Vault.Shared.Authentication.Pocket
{
    public static class PocketExtensions
    {
        public static AuthenticationBuilder AddPocket(this AuthenticationBuilder builder)
            => builder.AddPocket(PocketDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddPocket(this AuthenticationBuilder builder, Action<PocketOptions> configureOptions)
            => builder.AddPocket(PocketDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddPocket(this AuthenticationBuilder builder, string authenticationScheme, Action<PocketOptions> configureOptions)
            => builder.AddPocket(authenticationScheme, PocketDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddPocket(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<PocketOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<PocketOptions>, PocketPostConfigureOptions>());
            return builder.AddRemoteScheme<PocketOptions, PocketAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
