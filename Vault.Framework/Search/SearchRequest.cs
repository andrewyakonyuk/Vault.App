using System;
using System.Collections.Generic;
using System.Text;
using Vault.Framework.Search.Criteria;
using Vault.Shared;
using Vault.Shared.Queries;

namespace Vault.Framework.Search
{
    public class SearchRequest : ICriterion
    {
        public SearchRequest()
        {
            Criteria = new List<ISearchCriteria>();
        }

        public int Offset { get; set; }

        public int Count { get; set; }

        public int OwnerId { get; set; }

        public IList<ISearchCriteria> Criteria { get; set; }

        public ISearchResultTransformer ResultTransformer { get; set; }
    }
}