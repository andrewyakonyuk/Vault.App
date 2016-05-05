using Vault.Shared;

namespace Vault.Framework.Search
{
    public interface ISearchProvider
    {
        IPagedEnumerable<SearchDocument> Search(SearchRequest request);
    }
}