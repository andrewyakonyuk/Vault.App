using NHibernate;
using NHibernate.Context;
using System;

namespace Vault.Shared.NHibernate
{
    public class SessionProvider : ISessionProvider
    {
        private readonly ISessionFactory _sessionFactory;

        public SessionProvider(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public ISession CurrentSession
        {
            get
            {
                if (CurrentSessionContext.HasBind(_sessionFactory))
                    return _sessionFactory.GetCurrentSession();

                throw new InvalidOperationException(
                    "Database access logic cannot be used, if session not opened. Implicitly session usage not allowed now. Please open session explicitly through UnitOfWorkFactory.StartLongConversation method");
            }
        }
    }
}