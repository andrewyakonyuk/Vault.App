using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Vault.Activity;
using Vault.Activity.Commands;
using Vault.Activity.Resources;
using Vault.Activity.Services.Connectors;

namespace Vault.Shared.Connectors.Pocket
{
    public class PocketConnectionProvider : IPullConnectionProvider
    {
        readonly HttpClient _httpClient;
        readonly PocketConnectionOptions _options;
        const int DefaultCount = 50;
        static readonly DateTime Epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        const string PocketDataEndpoint = "https://getpocket.com/v3/get";

        public PocketConnectionProvider(IOptions<PocketConnectionOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
                throw new ArgumentNullException(nameof(optionsAccessor));
            if (optionsAccessor.Value == null)
                throw new ArgumentNullException(nameof(optionsAccessor.Value));

            _httpClient = new HttpClient();
            _options = optionsAccessor.Value;
        }

        public string Name { get { return "Pocket"; } }

        public async Task<PullResult> PullAsync(PullConnectionContext context)
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

            var activities = new List<ActivityCommandBase>(parsedResponse.Items.Count * 2);

            foreach (var parsedItem in parsedResponse.Items)
            {
                var key = new ResourceKey(parsedItem.ResolvedId, ResourceTypes.Article, Name, context.User.Id);
                var published = DateTimeOffset.UtcNow;
                var article = new ArticleResource
                {
                    Uri = new Uri(parsedItem.Uri, UriKind.Absolute)
                };

                activities.Add(new ReadActivityCommand<ArticleResource>(article, key,
                    published));

                if (parsedItem.IsFavorite)
                    activities.Add(new LikeActivityCommand<ArticleResource>(key, published));
                else activities.Add(new DislikeActivityCommand<ArticleResource>(key, published));
            }

            return new PullResult(activities, context.Batch) { IsCancellationRequested = parsedResponse.Items.Count == 0 };
        }
    }
}