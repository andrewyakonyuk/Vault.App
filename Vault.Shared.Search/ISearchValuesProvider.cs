using System.Collections.Generic;

namespace Vault.Shared.Search
{
    public interface ISearchValuesProvider
    {
        IReadOnlyList<string> GetValues(string key);
    }
}