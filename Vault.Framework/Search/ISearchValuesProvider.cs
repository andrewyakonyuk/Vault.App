using System.Collections.Generic;

namespace Vault.Framework.Search
{
    public interface ISearchValuesProvider
    {
        IReadOnlyList<string> GetValues(string key);
    }
}