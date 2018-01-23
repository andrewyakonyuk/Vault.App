using Vault.WebApp.Services.Security;

namespace Vault.WebApp.Services.Boards
{
    public class Permissions
    {
        public static readonly Permission ViewBoard = new Permission
        {
            Description = "View board",
            Name = "ViewBoard"
        };

        public static readonly Permission CreateBoard = new Permission
        {
            Description = "Create board",
            Name = "CreateBoard",
            RequiresOwnership = true
        };

        public static readonly Permission UpdateBoard = new Permission
        {
            Description = "Update board",
            Name = "UpdateBoard",
            RequiresOwnership = true
        };
    }
}