using Vault.Shared.Queries;

namespace Vault.Shared.Search.Criteria
{
    public interface ISearchCriteria : ICriterion
    {
        void Apply(ISearchCriteriaBuilder builder);
    }
}