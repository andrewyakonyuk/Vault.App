using System.Threading.Tasks;

namespace Vault.Shared.Queries
{
    public interface IQueryFor<T>
    {
        Task<T> With<TCriterion>(TCriterion criterion)
            where TCriterion : ICriterion;
    }
}