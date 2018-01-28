using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;
using Vault.Spouts.Pocket.Json;
using StreamInsights.Abstractions;
using Vault.Spouts.Abstractions;

namespace Vault.Spouts.Pocket
{
    public class PocketSpout : ISpout<Activity>
    {
        readonly HttpClient _httpClient;
        readonly PocketSpoutOptions _options;
        const int DefaultCount = 50;
        static readonly DateTime Epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        const string PocketDataEndpoint = "https://getpocket.com/v3/get";

        public PocketSpout(IOptions<PocketSpoutOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
                throw new ArgumentNullException(nameof(optionsAccessor));
            if (optionsAccessor.Value == null)
                throw new ArgumentNullException(nameof(optionsAccessor.Value));

            _httpClient = new HttpClient();
            _options = optionsAccessor.Value;
        }

        public string Name { get { return "Pocket"; } }

        public async Task<ConsumeResult<Activity>> ConsumeAsync(ConsumeMessageContext context)
        {
            var queryBuilder = new QueryBuilder();
            queryBuilder.Add("consumer_key", _options.ConsumerKey);
            queryBuilder.Add("access_token", context.User.AccessCode);
            queryBuilder.Add("count", DefaultCount.ToString());
            queryBuilder.Add("detailType", "complete");
            queryBuilder.Add("contentType", "article"); //todo: hardcode
            queryBuilder.Add("sort", "oldest");
            var offset = DefaultCount * context.Batch;
            queryBuilder.Add("offset", offset.ToString());

            if (context.LastFetchTimeUtc.HasValue)
            {
                var unixTime = (int)Math.Truncate(context.LastFetchTimeUtc.Value.Subtract(Epoc).TotalSeconds);
                queryBuilder.Add("since", unixTime.ToString());
            }

            var request = new HttpRequestMessage(HttpMethod.Get, PocketDataEndpoint + queryBuilder.ToString());

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            json = json.Replace("[]", "{}");
            var parsedResponse = JsonConvert.DeserializeObject<Retrieve>(json, new JsonSerializerSettings
            {
                Converters =
                {
                    new PocketItemConverter(),
                    new BoolConverter(),
                    new UnixDateTimeConverter(),
                    new NullableIntConverter(),
                    new UriConverter()
                }
            });

            var result = new List<Activity>(parsedResponse.Items.Count * 2);

            foreach (var parsedItem in parsedResponse.Items)
            {
                var published = parsedItem.TimeAdded.ToUniversalTime();

                var readActivity = new Activity
                {
                    Actor = new ASObject
                    {
                        Id = context.User.Id
                    },
                    Id = parsedItem.ResolvedId,
                    Generator = Name,
                    Type = "read",
                    Published = published,
                    Url = parsedItem.Uri,
                    Name = parsedItem.Title,
                    Summary = parsedItem.Excerpt,
                    Image = parsedItem.Image != null ? new ASObject
                    {
                        Url = parsedItem.Image?.Src,
                        Summary = parsedItem.Image?.Caption
                    } : null,
                    Tag = parsedItem.Tags?.Select(t=>t.Name).ToArray(),
                    StartTime = parsedItem.TimeAdded
                };

                var likeActivity = new Activity
                {
                    Actor = new ASObject
                    {
                        Id = context.User.Id
                    },
                    Id = parsedItem.ResolvedId,
                    Generator = Name,
                    Type = parsedItem.IsFavorite ? "like" : "unlike",
                    Published = published,
                    Url = parsedItem.Uri,
                    Name = parsedItem.Title,
                    Summary = parsedItem.Excerpt,
                    Image = parsedItem.Image != null ? new ASObject
                    {
                        Url = parsedItem.Image?.Src,
                        Summary = parsedItem.Image?.Caption
                    } : null,
                    Tag = parsedItem.Tags?.Select(t => t.Name).ToArray(),
                    StartTime = parsedItem.TimeAdded
                };

                result.Add(readActivity);
                result.Add(likeActivity);
            }

            return new ConsumeResult<Activity>(result, context.Batch) { IsCancellationRequested = parsedResponse.Items.Count == 0 };
        }
    }
}