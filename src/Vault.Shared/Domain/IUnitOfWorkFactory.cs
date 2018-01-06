using System.Data;

namespace Vault.Shared.Domain
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(IsolationLevel isolationLevel);

        IUnitOfWork Create();
    }
}