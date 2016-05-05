namespace Vault.Framework.Search.Criteria
{
    public interface ISearchFilterBuilder
    {
        ISearchFilterBuilder AddEqual(string fieldName, object value, bool not);

        ISearchFilterBuilder AddContains(string fieldName, object value, bool not);

        ISearchFilterBuilder AddBetween(string fieldName, object lowValue, object highValue, bool strict);

        object Build();
    }
}