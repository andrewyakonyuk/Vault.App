﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Connectors.Pocket.Json
{
    public class BoolConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((bool)value) ? 1 : 0);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value == null ? false : (reader.Value.ToString() == "1");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
    }
}