using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Vault.Shared.Activity;

namespace Vault.WebHost.Services.Activities
{
    public class RestActivityStream : IActivityStream
    {
        readonly string _bucket;
        readonly string _streamId;
        readonly HttpClient _httpClient;
        readonly ActivityClientOptions _options;

        public RestActivityStream(string bucket, string streamId, ActivityClientOptions options)
        {
            _bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
            _streamId = streamId ?? throw new ArgumentNullException(nameof(streamId));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpClient = new HttpClient();
        }

        public async Task PushActivityAsync(ActivityEventAttempt activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            string uri = $"{_options.WebserviceUri}/streams/{_bucket}/{_streamId}";
            var response = await _httpClient.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(activity), System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        public async Task<IReadOnlyCollection<CommitedActivityEvent>> ReadEventsAsync(string query, long checkpointToken, int maxCount)
        {
            string uri = $"{_options.WebserviceUri}/streams/{_bucket}/{_streamId}?checkpointToken={checkpointToken}&maxCount={maxCount}&query={query}";

            var dataString = await _httpClient.GetStringAsync(uri);
            var response = JsonConvert.DeserializeObject<IReadOnlyCollection<CommitedActivityEvent>>(dataString);
            return response;
        }
    }
}
