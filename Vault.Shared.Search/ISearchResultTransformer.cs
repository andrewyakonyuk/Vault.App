namespace Vault.Shared.Search
{
    public interface ISearchResultTransformer
    {
        SearchDocument Transform(ISearchValuesProvider valuesProvider);
    }
}