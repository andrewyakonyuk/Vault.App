namespace Vault.Shared.Search.Criteria
{
    public interface ISearchValue
    {
        Examineness Examineness { get; }

        float Level { get; }

        object Value { get; }
    }

    public sealed class SearchValue : ISearchValue
    {
        public SearchValue(Examineness vagueness, object value)
            : this(vagueness, value, 1f)
        {
        }

        public SearchValue(Examineness vagueness, object value, float level)
        {
            Examineness = vagueness;
            Value = value;
            Level = level;
        }

        public Examineness Examineness { get; private set; }

        public float Level { get; private set; }

        public object Value { get; private set; }
    }
}