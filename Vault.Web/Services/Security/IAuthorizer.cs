using Vault.Shared;

namespace Vault.WebHost.Services.Security
{
    /// <summary>
    /// Authorization services for the current user
    /// </summary>
    public interface IAuthorizer
    {
        /// <summary>
        /// Authorize the current user against a permission
        /// </summary>
        /// <param name="permission">A permission to authorize against</param>
        bool Authorize(Permission permission);

        /// <summary>
        /// Authorize the current user against a permission for a specified content item
        /// </summary>
        /// <param name="permission">A permission to authorize against</param>
        /// <param name="content">A content item the permission will be checked for</param>
        bool Authorize(Permission permission, IContent content);
    }
}