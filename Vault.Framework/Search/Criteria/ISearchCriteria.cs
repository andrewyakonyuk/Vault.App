using Vault.Shared.Queries;

namespace Vault.Framework.Search.Criteria
{
    public interface ISearchCriteria : ICriterion
    {
        void Apply(ISearchFilterBuilder builder);
    }
}