using NHibernate;
using NHibernate.Context;
using System;
using System.Data;
using Vault.Shared.Domain;

namespace Vault.Shared.NHibernate
{
    public class NHibernateUnitOfWork : IUnitOfWork
    {
        private readonly ISession _session;
        private ITransaction _transaction;

        public NHibernateUnitOfWork(ISession session, IsolationLevel isolationLevel)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            CurrentSessionContext.Bind(session);

            _session = session;
            _transaction = session.BeginTransaction(isolationLevel);
        }

        public void Dispose()
        {
            if (!_transaction.WasCommitted && !_transaction.WasRolledBack)
                _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;

            CurrentSessionContext.Unbind(_session.SessionFactory);
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Save(IEntity entity)
        {
            _session.Evict(entity);
            _session.SaveOrUpdate(entity);
        }

        public void Delete(IEntity entity)
        {
            _session.Evict(entity);
            _session.Delete(entity);
        }
    }
}