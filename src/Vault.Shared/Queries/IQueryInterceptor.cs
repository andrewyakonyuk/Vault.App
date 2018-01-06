using System.Threading.Tasks;

namespace Vault.Shared.Queries
{
    public interface IQueryInterceptor
    {
        int Order { get; }

        Task<TResult> AskAsync<TCriterion, TResult>(TCriterion criterion, AskQueryMethod<TCriterion, TResult> askMethod)
            where TCriterion : ICriterion;
    }

    public delegate Task<TResult> AskQueryMethod<in TCriterion, TResult>(TCriterion criterion)
        where TCriterion : ICriterion;
}
