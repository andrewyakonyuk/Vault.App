using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;

namespace Vault.Spouts.Pocket.Json
{
    public class PocketItemConverter : CustomCreationConverter<PocketItem>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var jObject = JObject.ReadFrom(reader);
            var pocketItem = new PocketItem();
            serializer.Populate(jObject.CreateReader(), pocketItem);

            return pocketItem;
        }

        public override PocketItem Create(Type objectType)
        {
            return new PocketItem();
        }
    }
}
