namespace Vault.Shared.Search
{
    public interface ISearchProvider
    {
        IPagedEnumerable<SearchDocument> Search(SearchRequest request);
    }
}