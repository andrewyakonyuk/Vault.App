using Lucene.Net.Documents;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Vault.Shared.Search.Lucene.Converters
{
    public class LuceneDateTimeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return DateTools.DateToString((DateTime)value, DateTools.Resolution.MILLISECOND);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return DateTools.StringToDate((string)value);
        }
    }
}