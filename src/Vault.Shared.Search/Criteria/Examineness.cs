namespace Vault.Shared.Search.Criteria
{
    public enum Examineness
    {
        Fuzzy,
        SimpleWildcard,
        ComplexWildcard,
        Explicit,
        Escaped,
        Boosted,
        Proximity
    }
}