using Vault.Framework.Identity;
using Vault.Shared;

namespace Vault.Framework.Security
{
    /// <summary>
    /// Entry-point for configured authorization scheme.
    /// </summary>
    public interface IAuthorizationService
    {
        bool TryCheckAccess(Permission permission, IUser user, IContent content);
    }
}