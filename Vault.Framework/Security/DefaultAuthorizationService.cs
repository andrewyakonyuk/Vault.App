using Vault.Framework.Identity;

namespace Vault.Framework.Security
{
    public class DefaultAuthorizationService : IAuthorizationService
    {
        public bool TryCheckAccess(Permission permission, IUser user, IContent content)
        {
            if (content == null)
                return true;

            return content.OwnerId == user?.Id;
        }
    }
}