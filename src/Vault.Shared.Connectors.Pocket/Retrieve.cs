using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vault.Shared.Connectors.Pocket.Json;

namespace Vault.Shared.Connectors.Pocket
{
    [JsonObject]
    public class Retrieve
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        /// <summary>
        /// Gets or sets the complete.
        /// </summary>
        /// <value>
        /// The complete.
        /// </value>
        [JsonProperty("complete")]
        public int Complete { get; set; }

        /// <summary>
        /// Gets or sets the since.
        /// </summary>
        /// <value>
        /// The since.
        /// </value>
        [JsonProperty("since")]
        public DateTime Since { get; set; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [JsonProperty("list")]
        [JsonConverter(typeof(ObjectToArrayConverter<PocketItem>))]
        public List<PocketItem> Items { get; set; }
    }
}
