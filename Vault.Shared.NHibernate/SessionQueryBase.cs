using NHibernate;
using System.Threading.Tasks;
using Vault.Shared.Queries;

namespace Vault.Shared.NHibernate
{
    public abstract class SessionQueryBase<TCriterion, TResult> : IQuery<TCriterion, TResult>
        where TCriterion : ICriterion
    {
        private readonly ISessionProvider _sessionProvider;

        protected SessionQueryBase(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        protected virtual ISession Session
        {
            get { return _sessionProvider.CurrentSession; }
        }

        public abstract Task<TResult> AskAsync(TCriterion criterion);
    }
}