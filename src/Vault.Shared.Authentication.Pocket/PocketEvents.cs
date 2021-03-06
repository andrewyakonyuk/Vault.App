﻿using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace Vault.Shared.Authentication.Pocket
{
    public class PocketEvents : RemoteAuthenticationEvents, IPocketEvents
    {
        public virtual Task RedirectToAuthorizationEndpoint(PocketRedirectToAuthorizationEndpointContext context)
        {
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    }
}