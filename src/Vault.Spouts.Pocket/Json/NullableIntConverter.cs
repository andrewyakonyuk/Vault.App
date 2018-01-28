using Newtonsoft.Json;
using System;

namespace Vault.Spouts.Pocket.Json
{
    public class NullableIntConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int result = 0;
            if (reader.Value != null)
            {
                result = Convert.ToInt32(reader.Value);
            }
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int);
        }
    }
}
