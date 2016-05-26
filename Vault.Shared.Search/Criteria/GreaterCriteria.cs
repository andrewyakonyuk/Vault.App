namespace Vault.Shared.Search
{
    public class GreaterCriteria : ISearchCriteria
    {
        public bool Strict { get; set; }

        public string FieldName { get; set; }

        public object Value { get; set; }

        public void Apply(ISearchFilterBuilder builder)
        {
            builder.AddBetween(FieldName, Value, null, Strict);
        }
    }
}