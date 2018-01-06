namespace Vault.Shared.Search.Criteria
{
    public interface ISearchCriteriaBuilder
    {
        IBooleanOperation Boolean();

        IBooleanOperation Field(string fieldName, ISearchValue value);

        IBooleanOperation Range(string fieldName, object lower, object upper, bool includeLower, bool includeUpper);

        IBooleanOperation Grouped(ISearchCriteriaBuilder group);
    }
}