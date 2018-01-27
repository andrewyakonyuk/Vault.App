using Vault.Shared.Domain;

namespace Vault.WebApp.Infrastructure.Identity
{
    public interface IUser : IEntity
    {
        string Email { get; set; }
        string UserName { get; set; }
    }
}