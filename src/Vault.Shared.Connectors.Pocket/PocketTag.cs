using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Connectors.Pocket
{
    /// <summary>
    /// Tag
    /// </summary>
    [JsonObject]
    public class PocketTag
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("tag")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the item iD.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("item_id")]
        public string ItemID { get; set; }
    }
}
