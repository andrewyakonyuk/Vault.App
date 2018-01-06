using Lucene.Net.Search;
using Vault.Shared.Search.Criteria;

namespace Vault.Shared.Search.Lucene.Criteria
{
    public class LuceneSearchCriteriaBuilderWrapper : ISearchCriteriaBuilder
    {
        readonly LuceneSearchCriteriaBuilder _builder;
        readonly Occur _occurance;

        public LuceneSearchCriteriaBuilderWrapper(LuceneSearchCriteriaBuilder builder, Occur occurance)
        {
            _builder = builder;
            _occurance = occurance;
        }

        public IBooleanOperation Boolean()
        {
            return _builder.Boolean();
        }

        public IBooleanOperation Field(string fieldName, ISearchValue value)
        {
            return _builder.Field(fieldName, value, _occurance);
        }

        public IBooleanOperation Grouped(ISearchCriteriaBuilder group)
        {
            return _builder.Grouped(group, _occurance);
        }

        public IBooleanOperation Range(string fieldName, object lower, object upper, bool includeLower, bool includeUpper)
        {
            return _builder.Range(fieldName, lower, upper, includeLower, includeUpper, _occurance);
        }
    }
}