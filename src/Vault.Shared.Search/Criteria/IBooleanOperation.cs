namespace Vault.Shared.Search.Criteria
{
    public interface IBooleanOperation
    {
        ISearchCriteriaBuilder And();

        ISearchCriteriaBuilder Or();

        ISearchCriteriaBuilder Not();
    }
}