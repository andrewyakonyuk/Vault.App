using NHibernate;
using System;

namespace Vault.Shared.NHibernate
{
    public class PerRequestSessionProvider : ISessionProvider, IDisposable
    {
        private readonly ISessionFactory _sessionFactory;
        private bool _disposed;
        private ISession _session;

        public PerRequestSessionProvider(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _session?.Dispose();
            _session = null;
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

                return _session;
            }
        }
    }
}