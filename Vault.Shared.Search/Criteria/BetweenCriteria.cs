namespace Vault.Shared.Search.Criteria
{
    public class BetweenCriteria : ISearchCriteria
    {
        public object Lower { get; set; }
        public object Upper { get; set; }
        public string FieldName { get; set; }
        public bool IncludeLower { get; set; }
        public bool IncludeUpper { get; set; }

        public void Apply(ISearchCriteriaBuilder builder)
        {
            builder.Range(FieldName, Lower, Upper, IncludeLower, IncludeUpper);
        }
    }
}