using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Vault.Framework.Search.Parsing
{
    public class DefaultSearchQueryParser : ISearchQueryParser
    {
        static readonly Regex WhitespaceRegex = new Regex(@"\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex SearchTermsRegex = new Regex(@"(\S+:'(?:[^'\\]|\\.)*')|(\S+:""(?:[^ ""\\]|\\.)*"") |\S+|\S+:\S+", RegexOptions.Compiled);
        static readonly Regex StripSurroundingQuotes = new Regex(@"^\""|\""$|^\'|\'$", RegexOptions.Compiled);

        static readonly Dictionary<string, string> InvertedGroupTypes = new Dictionary<string, string>
        {
            {"=", "!=" },
            { ">=", "<" },
            { ">", "<=" },
            {"!=", "=" },
            {"<=", ">" },
            {"<", ">=" },
            {"in", "nin" },
            {"nin", "in" }
        };

        static readonly string[] GroupTypes = new[] { "=", ">=", ">", "!=", "<=", "<" };

        const string NotType = "NOT";
        const string DefaultGroupType = "in";
        const string FieldDelimenter = ":";

        public SearchQueryTokenStream Parse(string input)
        {
            if (string.IsNullOrEmpty(input))
                return SearchQueryTokenStream.Empty;

            // Regularize white spacing
            // Make in-between white spaces a unique space
            input = WhitespaceRegex.Replace(input, " ");

            // When a simple string, return it
            if (input.IndexOf(FieldDelimenter) == -1)
                return new SearchQueryTokenStream(input, new SearchQueryToken[]
                {
                    new SearchQueryToken("keywords", "in", input)
                });

            // Otherwise parse the advanced query syntax
            else
            {
                // Get a list of search terms respecting single and double quotes
                var matches = SearchTermsRegex.Matches(input);
                var terms = new List<string>(matches.Count);
                foreach (Match match in matches)
                {
                    var sepIndex = match.Value.IndexOf(FieldDelimenter);
                    if (sepIndex != -1)
                    {
                        var key = match.Value.Substring(0, sepIndex);
                        var value = match.Value.Substring(sepIndex + 1);

                        // Strip surrounding quotes
                        value = StripSurroundingQuotes.Replace(value, string.Empty);
                        value = value.Trim();
                        terms.Add(string.Join(FieldDelimenter, key, value));
                    }
                    else terms.Add(match.Value);
                }

                var tokens = new List<SearchQueryToken>(terms.Count);
                foreach (var term in terms)
                {
                    // Advanced search terms syntax has key and value
                    // separated with a colon
                    var sepIndex = term.IndexOf(FieldDelimenter);
                    // When just a simple term
                    if (sepIndex == -1)
                    {
                        //case sensetive
                        if (term == NotType)
                        {
                            tokens.Add(new SearchQueryToken(null, NotType, null));
                        }
                        else
                        {
                            // We add it as pure text
                            tokens.Add(new SearchQueryToken("keywords", "in", term));
                        }
                    }
                    // We got an advanced search syntax
                    else
                    {
                        var key = term.Substring(0, sepIndex);
                        var value = term.Substring(sepIndex + 1);
                        var type = DetectGroupType(value) ?? DefaultGroupType;

                        var typeIndex = value.IndexOf(type);
                        if (typeIndex != -1)
                            value = value.Substring(typeIndex + type.Length);

                        tokens.Add(new SearchQueryToken(key, type, value));
                    }
                }

                var invert = false;
                var resultTokens = new List<SearchQueryToken>(tokens.Count);
                foreach (var token in tokens)
                {
                    if (token.Type == NotType)
                    {
                        // Eliminate 'not' from groups
                        // Reverse next group
                        invert = true;
                    }
                    else if (invert)
                    {
                        var invertedGroup = new SearchQueryToken(token.FieldName, InvertedGroupTypes[token.Type], token.RawValue);
                        resultTokens.Add(invertedGroup);
                        invert = false;
                    }
                    else
                    {
                        resultTokens.Add(token);
                    }
                }

                return new SearchQueryTokenStream(input, resultTokens);
            }
        }

        string DetectGroupType(string value)
        {
            for (int i = 0; i < GroupTypes.Length; i++)
            {
                var type = GroupTypes[i];
                if (value.IndexOf(type) == 0)
                    return type;
            }
            return null;
        }
    }
}