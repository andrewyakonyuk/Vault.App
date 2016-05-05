namespace Vault.Framework.Search.Criteria
{
    public class LessCriteria : ISearchCriteria
    {
        public bool Strict { get; set; }

        public string FieldName { get; set; }

        public object Value { get; set; }

        public void Apply(ISearchFilterBuilder builder)
        {
            builder.AddBetween(FieldName, null, Value, Strict);
        }
    }
}