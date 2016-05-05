using NHibernate;
using System;

namespace Vault.Shared.NHibernate
{
    public class PerRequestSessionProvider : ISessionProvider, IDisposable
    {
        private readonly ISessionFactory _sessionFactory;
        private bool _disposed;
        private bool _preventCommit;
        private ISession _session;
        private ITransaction _transaction;

        public PerRequestSessionProvider(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            if (_session == null)
                return;

            try
            {
                if (_preventCommit)
                    _transaction.Rollback();
                else
                    _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
            }

            _session.Dispose();
            _session = null;
            _transaction = null;
            _disposed = true;
        }

        public ISession CurrentSession
        {
            get
            {
                if (_disposed)
                    throw new InvalidOperationException(
                        "Object already disposed. Probably container has wrong lifestyle type");

                if (_session != null)
                    return _session;

                _session = _sessionFactory.OpenSession();
                _transaction = _session.BeginTransaction();

                return _session;
            }
        }

        public void PreventCommit()
        {
            _preventCommit = true;
        }
    }
}