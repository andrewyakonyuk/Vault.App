using System.Collections.Generic;

namespace Vault.Framework.Search
{
    public interface ISearchValuesProvider
    {
        IEnumerable<string> GetValues(string key);
    }
}