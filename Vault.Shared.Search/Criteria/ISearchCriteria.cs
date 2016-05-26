using Vault.Shared.Queries;

namespace Vault.Shared.Search
{
    public interface ISearchCriteria : ICriterion
    {
        void Apply(ISearchFilterBuilder builder);
    }
}