using Microsoft.AspNet.Http.Extensions;
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Vault.Domain.Activities;
using Vault.Domain.Models;

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

        public async Task<PullConnectionResult> PullAsync(PullConnectionContext context)
        {
            if (context.CancellationToken.IsCancellationRequested)
                return PullConnectionResult.Empty;

            var queryBuilder = new QueryBuilder();
            queryBuilder.Add("consumer_key", _options.ConsumerKey);
            queryBuilder.Add("access_token", context.User.AccessCode);
            queryBuilder.Add("count", DefaultCount.ToString());
            queryBuilder.Add("detailType", "complete");
            queryBuilder.Add("contentType", "article"); //todo: hardcode
            queryBuilder.Add("sort", "oldest");
            var offset = DefaultCount * context.Iteration;
            queryBuilder.Add("offset", offset.ToString());

            if (context.LastRunTimeUtc.HasValue)
            {
                var unixTime = (int)Math.Truncate(context.LastRunTimeUtc.Value.Subtract(Epoc).TotalSeconds);
                queryBuilder.Add("since", unixTime.ToString());
            }

            var request = new HttpRequestMessage(HttpMethod.Get, PocketDataEndpoint + queryBuilder.ToString());

            var response = await _httpClient.SendAsync(request, context.CancellationToken);
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

            var activities = new List<Activity>(parsedResponse.Items.Count);

            foreach (var parsedItem in parsedResponse.Items)
            {
                var article = new Article
                {
                    AdditionalType = "Pocket",
                    Summary = parsedItem.Excerpt,
                    Description = parsedItem.Excerpt,
                    Name = parsedItem.Title,
                    Url = parsedItem.Uri
                };

                if (parsedItem.Image != null)
                {
                    article.Image = new Image
                    {
                        Caption = parsedItem.Image.Caption,
                        Height = parsedItem.Image.Height,
                        Width = parsedItem.Image.Width,
                        Url = parsedItem.Image.Src
                    };
                }

                activities.Add(new ReadActivity
                {
                    Affected = article,
                    Status = ActionStatusType.Completed,
                    Target = new EntryPoint
                    {
                        ActionPlatform = "Pocket",
                        ContentType = "text/html",
                        HttpMethod = HttpMethod.Get.Method,
                        UrlTemplate = $"https://getpocket.com/a/read/{parsedItem.ItemId}"
                    }
                });
            }

            return new PullConnectionResult(activities) { IsCancellationRequested = parsedResponse.Items.Count == 0 };
        }
    }
}