using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Vault.Framework.Search
{
    public class DefaultSearchQueryParser : ISearchQueryParser
    {
        static Regex WhitespaceRegex = new Regex(@"\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex SearchTermsRegex = new Regex(@"(\S+:'(?:[^'\\]|\\.)*')|(\S+:""(?:[^ ""\\]|\\.)*"") |\S+|\S+:\S+", RegexOptions.Compiled);
        static Regex StripSurroundingQuotes = new Regex(@"^\""|\""$|^\'|\'$", RegexOptions.Compiled);
        const string FieldDelimenter = ":";

        static Dictionary<string, string> InvertedGroupTypes = new Dictionary<string, string>
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

        static string[] GroupTypes = new[] { "=", ">=", ">", "!=", "<=", "<" };

        static string NotType = "NOT";
        const string DefaultGroupType = "in";

        public SearchQueryParserResult Parse(string input)
        {
            if (string.IsNullOrEmpty(input))
                return SearchQueryParserResult.Empty;

            input = input.ToLower();

            // Regularize white spacing
            // Make in-between white spaces a unique space
            input = WhitespaceRegex.Replace(input, " ");

            // When a simple string, return it
            if (input.IndexOf(FieldDelimenter) == -1)
                return new SearchQueryParserResult(input, new List<SearchQueryGroup>
                {
                    new SearchQueryGroup {
                        Field = "keywords",
                        Type = "in",
                        Value = input
                    }
                });

            // Otherwise parse the advanced query syntax
            else
            {
                // Get a list of search terms respecting single and double quotes
                var terms = new List<string>();
                foreach (Match item in SearchTermsRegex.Matches(input))
                {
                    var sepIndex = item.Value.IndexOf(FieldDelimenter);
                    if (sepIndex != -1)
                    {
                        var key = item.Value.Substring(0, sepIndex);
                        var value = item.Value.Substring(sepIndex + 1);

                        // Strip surrounding quotes
                        value = StripSurroundingQuotes.Replace(value, string.Empty);
                        value = value.Trim();
                        terms.Add(string.Join(FieldDelimenter, key, value));
                    }
                    else terms.Add(item.Value);
                }

                var parseGroups = new List<SearchQueryGroup>(terms.Count);
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
                            parseGroups.Add(new SearchQueryGroup
                            {
                                Type = NotType
                            });
                        }
                        else
                        {
                            // We add it as pure text
                            parseGroups.Add(new SearchQueryGroup("keywords", "in", term));
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

                        parseGroups.Add(new SearchQueryGroup(key, type, value));
                    }
                }

                // Eliminate 'not' from groups
                // Reverse next group
                var invert = false;
                var result = new List<SearchQueryGroup>(parseGroups.Count);

                foreach (var group in parseGroups)
                {
                    if (group.Type == NotType)
                    {
                        invert = true;
                    }
                    else if (invert)
                    {
                        group.Type = InvertedGroupTypes[group.Type];
                        invert = false;
                        result.Add(group);
                    }
                    else
                    {
                        result.Add(group);
                    }
                }

                return new SearchQueryParserResult(input, result);
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