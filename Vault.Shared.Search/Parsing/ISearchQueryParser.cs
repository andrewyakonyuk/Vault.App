namespace Vault.Shared.Search.Parsing
{
    /// <summary>
    /// Advanced search query parser
    /// </summary>
    public interface ISearchQueryParser
    {
        /// <summary>
        /// Parse the input string with advanced search query syntax
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        SearchQueryTokenStream Parse(string query);
    }
}