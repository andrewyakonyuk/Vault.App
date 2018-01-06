using System;

namespace Vault.Shared.Search.Criteria
{
    [Serializable]
    public class EqualCriteria : ISearchCriteria
    {
        public object Value { get; set; }
        public string FieldName { get; set; }

        public bool Not { get; set; }

        public void Apply(ISearchCriteriaBuilder builder)
        {
            var criteriaBuilder = builder;
            if (Not)
                criteriaBuilder = builder.Boolean().Not();
            criteriaBuilder.Field(FieldName, new SearchValue(Examineness.Explicit, Value));
        }
    }
}