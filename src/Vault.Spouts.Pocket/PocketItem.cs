using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vault.Spouts.Pocket.Json;

namespace Vault.Spouts.Pocket
{
    [JsonObject]
    [DebuggerDisplay("Uri = {Uri}, Title = {Title}")]
    public class PocketItem
    {
        public PocketItem()
        {
            Tags = new List<PocketTag>();
        }

        [JsonProperty("item_id")]
        public string ItemId { get; set; }

        [JsonProperty("resolved_id")]
        public string ResolvedId { get; set; }

        [JsonProperty("given_url")]
        public string GivenUrl { get; set; }

        [JsonProperty("resolved_url")]
        public string ResolvedUrl { get; set; }

        [JsonProperty("given_title")]
        public string GivenTitle { get; set; }

        [JsonProperty("resolved_title")]
        public string ResolvedTitle { get; set; }

        [JsonProperty("favorite")]
        public bool IsFavorite { get; set; }

        [JsonProperty("word_count")]
        public int WordsCount { get; set; }

        [JsonProperty("excerpt")]
        public string Excerpt { get; set; }

        [JsonIgnore]
        public string Title { get { return string.IsNullOrEmpty(GivenTitle) ? ResolvedTitle : GivenTitle; } }

        [JsonIgnore]
        public string Uri { get { return string.IsNullOrEmpty(GivenUrl) ? ResolvedUrl : GivenUrl; } }

        [JsonProperty("image")]
        public PocketImage Image { get; set; }

        [JsonProperty("tags")]
        [JsonConverter(typeof(ObjectToArrayConverter<PocketTag>))]
        public IEnumerable<PocketTag> Tags { get; set; }

        [JsonProperty("time_added")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime TimeAdded { get; set; }
    }
}