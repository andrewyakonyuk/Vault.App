namespace Vault.Shared.Queries
{
    public interface IQueryBuilder
    {
        IQueryFor<TResult> For<TResult>();
    }
}