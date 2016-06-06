﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Vault.Shared.Connectors.Pocket
{
    [JsonObject]
    [DebuggerDisplay("Uri = {Uri}, Title = {Title}")]
    public class PocketItem
    {
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
    }
}
