﻿using Vault.Shared;
using Vault.Shared.Identity;

namespace Vault.WebHost.Services.Security
{
    /// <summary>
    /// Entry-point for configured authorization scheme.
    /// </summary>
    public interface IAuthorizationService
    {
        bool TryCheckAccess(Permission permission, IUser user, IContent content);
    }
}