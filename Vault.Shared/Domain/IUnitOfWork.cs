using System;

namespace Vault.Shared.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();

        void Save(IEntity entity);

        void Delete(IEntity entity);
    }
}