using System;

namespace Vault.Shared.Search.Criteria
{
    [Serializable]
    class LessCriteria : ISearchCriteria
    {
        public bool Strict { get; set; }

        public string FieldName { get; set; }

        public object Value { get; set; }

        public void Apply(ISearchCriteriaBuilder builder)
        {
            builder.Range(FieldName, null, Value, false, true);
        }
    }
}