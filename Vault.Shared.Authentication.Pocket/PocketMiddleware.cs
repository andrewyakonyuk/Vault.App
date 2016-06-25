using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;
using System;
using System.Net.Http;
using System.Text.Encodings.Web;

namespace Vault.Shared.Authentication.Pocket
{
    public class PocketMiddleware : AuthenticationMiddleware<PocketOptions>
    {
        private readonly HttpClient _httpClient;

        public PocketMiddleware(
            RequestDelegate next,
            IDataProtectionProvider dataProtectionProvider,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            IOptions<PocketOptions> options)
            : base(next, options, loggerFactory, encoder)
        {
            if (string.IsNullOrEmpty(options.Value.ConsumerKey))
                throw new ArgumentException("ConsumerKey must be specified");

            _httpClient = new HttpClient(options.Value.BackchannelHttpHandler ?? new HttpClientHandler());
            _httpClient.Timeout = options.Value.BackchannelTimeout;
            _httpClient.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB
            _httpClient.DefaultRequestHeaders.Accept.ParseAdd("*/*");
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Pocket authentication middleware");
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;

            if (Options.Events == null)
            {
                Options.Events = new PocketEvents();
            }

            if (Options.StateDataFormat == null)
            {
                var dataProtector = dataProtectionProvider.CreateProtector(
                    typeof(PocketMiddleware).FullName, Options.AuthenticationScheme, "v1");
                Options.StateDataFormat = new SecureDataFormat<RequestToken>(
                    new RequestTokenSerializer(),
                    dataProtector);
            }
        }

        protected override AuthenticationHandler<PocketOptions> CreateHandler()
        {
            return new PocketAuthenticationHandler(_httpClient);
        }
    }
}