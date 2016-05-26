using System.Collections.Generic;
using Vault.Shared.Queries;

namespace Vault.Shared.Search
{
    public class SearchRequest : ICriterion
    {
        public SearchRequest()
        {
            Criteria = new List<ISearchCriteria>();
            SortBy = new List<SortField>();
        }

        public int Offset { get; set; }

        public int Count { get; set; }

        public int OwnerId { get; set; }

        public IList<ISearchCriteria> Criteria { get; set; }

        public ISearchResultTransformer ResultTransformer { get; set; }

        public IList<SortField> SortBy { get; set; }
    }

    public class SortField
    {
        public string FieldName { get; private set; }
        public bool Ascending { get; private set; }

        public SortField(string fieldName, bool ascending)
        {
            FieldName = fieldName;
            Ascending = ascending;
        }
    }
}