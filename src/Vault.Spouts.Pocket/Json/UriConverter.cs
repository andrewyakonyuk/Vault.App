using Newtonsoft.Json;
using System;

namespace Vault.Spouts.Pocket.Json
{
    public class UriConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                return null;
            }

            string value = reader.Value.ToString();

            if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
            {
                return new Uri(value);
            }
            else if (value.StartsWith("//") && Uri.IsWellFormedUriString("http:" + value, UriKind.Absolute))
            {
                return new Uri("http:" + value);
            }
            else if (value.StartsWith("www.") && Uri.IsWellFormedUriString("http://" + value, UriKind.Absolute))
            {
                return new Uri("http://" + value);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else if (value is Uri)
            {
                writer.WriteValue(((Uri)value).OriginalString);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(Uri));
        }
    }
}
