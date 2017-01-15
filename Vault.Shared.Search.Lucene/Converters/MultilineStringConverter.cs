using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Vault.Shared.Search.Lucene.Converters
{
    public class MultilineStringConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType) || destinationType.IsAssignableFrom(typeof(IEnumerable<string>));
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return string.Empty;

            return string.Join(Environment.NewLine, (IEnumerable<string>)value);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            var strValue = (string)value;
            var splitedValue = strValue.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            if (splitedValue.Length == 0)
                return null;

            return new List<string>(splitedValue);
        }
    }
}
