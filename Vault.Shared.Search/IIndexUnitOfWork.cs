using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Search
{
    public interface IIndexUnitOfWork : IDisposable
    {
        void Commit();

        void Save(SearchDocument entity);

        void Delete(SearchDocument entity);
    }
}