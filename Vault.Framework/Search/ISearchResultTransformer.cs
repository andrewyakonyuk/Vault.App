namespace Vault.Framework.Search
{
    public interface ISearchResultTransformer
    {
        SearchDocument Transform(ISearchValuesProvider valuesProvider);
    }
}