using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Vault.Shared.Activity;

namespace Vault.WebApp.Services.Activities
{
    public class RestActivityClient : IActivityClient
    {
        readonly ActivityClientOptions _options;

        public RestActivityClient(IOptions<ActivityClientOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options.Value;
        }

        public Task<IActivityStream> GetStreamAsync(string bucket, string streamId)
        {
            if (string.IsNullOrEmpty(bucket))
                throw new ArgumentNullException(nameof(bucket));
            if (string.IsNullOrEmpty(streamId))
                throw new ArgumentNullException(nameof(streamId));

            var stream = new RestActivityStream(bucket, streamId, _options);
            return Task.FromResult<IActivityStream>(stream);
        }
    }
}
