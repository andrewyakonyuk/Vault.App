using NHibernate;
using System.Data;
using Vault.Shared.Domain;

namespace Vault.Shared.NHibernate
{
    public class NHibernateUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ISessionProvider _sessionProvider;

        public NHibernateUnitOfWorkFactory(ISessionProvider secctionProvider)
        {
            _sessionProvider = secctionProvider;
        }

        public IUnitOfWork Create(IsolationLevel isolationLevel)
        {
            return new NHibernateUnitOfWork(_sessionProvider.CurrentSession, isolationLevel);
        }

        public IUnitOfWork Create()
        {
            return Create(IsolationLevel.ReadCommitted);
        }
    }
}