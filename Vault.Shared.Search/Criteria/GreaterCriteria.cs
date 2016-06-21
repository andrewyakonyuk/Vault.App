namespace Vault.Shared.Search.Criteria
{
    public class GreaterCriteria : ISearchCriteria
    {
        public bool Strict { get; set; }

        public string FieldName { get; set; }

        public object Value { get; set; }

        public void Apply(ISearchCriteriaBuilder builder)
        {
            builder.Range(FieldName, Value, null, true, false);
        }
    }
}