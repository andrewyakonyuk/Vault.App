using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Vault.WebHost.Mvc.Routing.Projections
{
    public class DashedRouteProjection : IRouteProjection
    {
        private static HashSet<UnicodeCategory> wordCharCategories = new HashSet<UnicodeCategory>()
        {
            UnicodeCategory.LowercaseLetter, UnicodeCategory.UppercaseLetter,
            UnicodeCategory.OtherLetter, UnicodeCategory.ConnectorPunctuation,
            UnicodeCategory.ModifierLetter, UnicodeCategory.TitlecaseLetter,
            UnicodeCategory.DecimalDigitNumber
        };

        private HashSet<char> _extraValidChars;
        private int _maxLength;
        private string _defaultValue;

        public DashedRouteProjection(bool allowSlash, int maxLength = 0, string defaultValue = null)
        {
            _extraValidChars = allowSlash ? new HashSet<char> { '/' } : new HashSet<char>();
            _maxLength = maxLength;
            _defaultValue = defaultValue;
        }

        public void Incoming(string key, IDictionary<string, object> values)
        {
        }

        public void Outgoing(string key, IDictionary<string, object> values)
        {
            if (!values.ContainsKey(key))
                return;

            var value = (string)values[key];
            value = DashedValue(value, _extraValidChars, _maxLength);
            if (String.IsNullOrEmpty(value))
                value = _defaultValue;
            values[key] = value;
        }

        private static string DashedValue(string value, HashSet<char> extraValidChars, int maxLength = 0)
        {
            if (String.IsNullOrEmpty(value))
                return value;

            var sb = new StringBuilder();
            bool isPrevSep = true;
            foreach (var ch in value)
            {
                if (wordCharCategories.Contains(Char.GetUnicodeCategory(ch)))
                {
                    sb.Append(ch);
                    isPrevSep = false;
                }
                else if (extraValidChars.Contains(ch))
                {
                    bool isPrevDash = isPrevSep && sb[sb.Length - 1] == '-';
                    if (isPrevDash)
                        sb.Length--;
                    if (!isPrevSep || isPrevDash)
                        sb.Append(ch);
                    isPrevSep = true;
                }
                else if (!isPrevSep)
                {
                    sb.Append('-');
                    isPrevSep = true;
                }
            }
            if (sb.Length > 0 && isPrevSep)
                sb.Length--;

            var s = sb.ToString();
            if (maxLength > 0 && sb.Length > maxLength)
            {
                s = s.Substring(0, maxLength);
                if (sb[maxLength] != '-')
                {
                    var prevDashIndex = s.LastIndexOf('-');
                    if (prevDashIndex > 0)
                        s = s.Remove(prevDashIndex);
                }
            }
            return s.Trim('_');
        }
    }
}