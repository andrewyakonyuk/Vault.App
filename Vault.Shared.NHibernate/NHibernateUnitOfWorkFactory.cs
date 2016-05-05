using NHibernate;
using System.Data;
using Vault.Shared.Domain;

namespace Vault.Shared.NHibernate
{
    public class NHibernateUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ISessionFactory _sessionSessionFactory;

        public NHibernateUnitOfWorkFactory(ISessionFactory sessionFactory)
        {
            _sessionSessionFactory = sessionFactory;
        }

        public IUnitOfWork Create(IsolationLevel isolationLevel)
        {
            return new NHibernateUnitOfWork(_sessionSessionFactory.OpenSession(), isolationLevel);
        }

        public IUnitOfWork Create()
        {
            return Create(IsolationLevel.ReadCommitted);
        }
    }
}