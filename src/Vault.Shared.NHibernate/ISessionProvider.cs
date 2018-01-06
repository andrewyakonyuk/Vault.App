using NHibernate;

namespace Vault.Shared.NHibernate
{
    public interface ISessionProvider
    {
        ISession CurrentSession { get; }
    }
}