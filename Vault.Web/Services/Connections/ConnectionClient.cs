using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vault.WebHost.Services.Connections
{
    public class ConnectionClient
    {
        readonly WebserviceOptions _options;
        readonly HttpClient _httpClient;

        public ConnectionClient(IOptions<WebserviceOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options.Value;
            _httpClient = new HttpClient();
        }

        public async Task PullAsync(ExternalLoginInfo login, string ownerId)
        {
            if (login == null)
                throw new ArgumentNullException(nameof(login));

            string uri = $"{_options.WebserviceUri}/system/pull";
            var response = await _httpClient.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(new
            {
                ProviderName = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                OwnerId = ownerId
            }), System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }
    }
}
