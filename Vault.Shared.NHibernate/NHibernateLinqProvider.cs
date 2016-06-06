using NHibernate.Linq;
using System;
using System.Linq;
using Vault.Shared.Domain;

namespace Vault.Shared.NHibernate
{
    public class NHibernateLinqProvider : ILinqProvider
    {
        private readonly ISessionProvider _sessionProvider;

        public NHibernateLinqProvider(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public void Dispose()
        {
        }

        public IQueryable<TEntity> Query<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return _sessionProvider.CurrentSession.Query<TEntity>();
        }
    }
}