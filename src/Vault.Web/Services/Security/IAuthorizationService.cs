using Vault.Shared;
using Vault.WebApp.Infrastructure.Identity;

namespace Vault.WebApp.Services.Security
{
    /// <summary>
    /// Entry-point for configured authorization scheme.
    /// </summary>
    public interface IAuthorizationService
    {
        bool TryCheckAccess(Permission permission, IUser user, IContent content);
    }
}