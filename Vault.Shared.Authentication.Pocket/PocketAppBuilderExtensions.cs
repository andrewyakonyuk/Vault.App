﻿using System;
using Vault.Shared.Authentication.Pocket;

namespace Microsoft.AspNet.Builder
{
    public static class PocketAppBuilderExtensions
    {
        public static IApplicationBuilder UsePocketAuthentication(this IApplicationBuilder app, Action<PocketOptions> configureOptions = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            var pocketOptions = new PocketOptions();
            configureOptions?.Invoke(pocketOptions);
            return app.UsePocketAuthentication(pocketOptions);
        }

        public static IApplicationBuilder UsePocketAuthentication(this IApplicationBuilder app, PocketOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return UseMiddlewareExtensions.UseMiddleware<PocketMiddleware>(app, options);
        }
    }
}