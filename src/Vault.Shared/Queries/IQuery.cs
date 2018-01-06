using System.Threading.Tasks;

namespace Vault.Shared.Queries
{
    public interface IQuery<in TCriterion, TResult>
        where TCriterion : ICriterion
    {
        Task<TResult> AskAsync(TCriterion criterion);
    }
}