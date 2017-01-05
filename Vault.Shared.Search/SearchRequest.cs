using System;
using System.Collections.Generic;
using Vault.Shared.Queries;
using Vault.Shared.Search.Criteria;

namespace Vault.Shared.Search
{
    [Serializable]
    public class SearchRequest : ICriterion
    {
        public SearchRequest()
        {
            Criteria = new List<ISearchCriteria>();
            SortBy = new List<SortField>();
            IndexName = IndexNames.Default;
        }

        public string IndexName { get; set; }

        public int Offset { get; set; }

        public int Count { get; set; }

        public string OwnerId { get; set; }

        public IList<ISearchCriteria> Criteria { get; set; }

        public ISearchResultTransformer ResultTransformer { get; set; }

        public IList<SortField> SortBy { get; set; }
    }

    [Serializable]
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