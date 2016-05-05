using NHibernate;
using System;

namespace Vault.Shared.NHibernate
{
    public class StaticSessionProvider : ISessionProvider
    {
        private readonly object _lockObject = new object();
        private readonly ISession _session;

        public StaticSessionProvider(ISession session)
        {
            _session = session;
        }

        public ISession CurrentSession
        {
            get
            {
                lock (_lockObject)
                {
                    return _session;
                }
            }
        }
    }
}