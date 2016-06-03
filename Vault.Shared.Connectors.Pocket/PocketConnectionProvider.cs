using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNet.Http.Extensions;

namespace Vault.Shared.Connectors.Pocket
{
    public class PocketConnectionProvider : IPullConnectionProvider
    {
        readonly HttpClient _httpClient;
        readonly PocketConnectionOptions _options;
        const int DefaultCount = 10;
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
            try
            {
                if (context.CancellationToken.IsCancellationRequested)
                    return PullConnectionResult.Empty;

                var queryBuilder = new QueryBuilder();
                queryBuilder.Add("consumer_key", _options.ConsumerKey);
                queryBuilder.Add("access_token", context.User.AccessCode);
                queryBuilder.Add("count", DefaultCount.ToString());
                var offset = DefaultCount * context.Iteration;
                queryBuilder.Add("offset", offset.ToString());

                if (context.LastRunTime.HasValue)
                {
                    var unitTime = (int)Math.Truncate(context.LastRunTime.Value.Subtract(Epoc).TotalSeconds);
                    queryBuilder.Add("since", unitTime.ToString());
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
                
                return new PullConnectionResult { IsCancellationRequested = true };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
