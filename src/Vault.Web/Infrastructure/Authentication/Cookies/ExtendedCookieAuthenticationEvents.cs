using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.WebApp.Infrastructure.Mvc;

namespace Vault.WebApp.Infrastructure.Authentication.Cookies
{
    public class ExtendedCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            if (context.Request.IsAjaxRequest())
            {
                context.Response.Headers["Location"] = context.RedirectUri;
                context.Response.StatusCode = 401;
            }
            else if (HttpMethods.IsGet(context.Request.Method))
            {
                context.Response.Redirect(context.RedirectUri);
            }
            else
            {
                context.Response.Redirect(BuildRedirectUri(context.Request, context.Options.LoginPath));
            }
            return Task.CompletedTask;
        }

        string BuildRedirectUri(HttpRequest request, string targetPath)
                    => request.Scheme + "://" + request.Host + request.PathBase + targetPath;

    }
}
