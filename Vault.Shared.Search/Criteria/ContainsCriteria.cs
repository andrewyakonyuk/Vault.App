namespace Vault.Shared.Search
{
    public class ContainsCriteria : ISearchCriteria
    {
        public object Value { get; set; }
        public string FieldName { get; set; }

        public bool Not { get; set; }

        public void Apply(ISearchFilterBuilder builder)
        {
            builder.AddContains(FieldName, Value, Not);
        }
    }
}