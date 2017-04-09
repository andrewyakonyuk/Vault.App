using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Vault.Shared.Connectors.Pocket.Json
{
    public class UnixDateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime date = ((DateTime)value).ToUniversalTime();
            DateTime epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var delta = date.Subtract(epoc);

            writer.WriteValue((int)Math.Truncate(delta.TotalSeconds));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value.ToString() == "0")
            {
                return null;
            }

            if (reader.Value.ToString().StartsWith("-"))
            {
                return null;
            }

            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToDouble(reader.Value));
        }
    }
}
