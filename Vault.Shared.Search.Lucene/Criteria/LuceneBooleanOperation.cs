using Lucene.Net.Search;
using System;
using Vault.Shared.Search.Criteria;

namespace Vault.Shared.Search.Lucene.Criteria
{
    public class LuceneBooleanOperation : IBooleanOperation
    {
        private readonly LuceneSearchCriteriaBuilder _builder;

        internal LuceneBooleanOperation(LuceneSearchCriteriaBuilder builder)
        {
            this._builder = builder;
        }

        public ISearchCriteriaBuilder And()
        {
            return new LuceneSearchCriteriaBuilderWrapper(_builder, Occur.MUST);
        }

        public ISearchCriteriaBuilder Not()
        {
            return new LuceneSearchCriteriaBuilderWrapper(_builder, Occur.MUST_NOT);
        }

        public ISearchCriteriaBuilder Or()
        {
            return new LuceneSearchCriteriaBuilderWrapper(_builder, Occur.SHOULD);
        }
    }
}