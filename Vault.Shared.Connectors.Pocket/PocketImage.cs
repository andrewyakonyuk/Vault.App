using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Connectors.Pocket
{
    [JsonObject]
    public class PocketImage
    {
        [JsonProperty("item_id")]
        public string ItemId { get; set; }

        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }
    }
}