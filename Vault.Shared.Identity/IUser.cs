using Vault.Shared.Domain;

namespace Vault.Shared.Identity
{
    public interface IUser : IEntity
    {
        string Email { get; set; }
        string UserName { get; set; }
    }
}