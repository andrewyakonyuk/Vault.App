using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Domain
{
    public interface ILinqProvider : IDisposable
    {
        IQueryable<TEntity> Query<TEntity>()
            where TEntity : class, IEntity, new();
    }
}
