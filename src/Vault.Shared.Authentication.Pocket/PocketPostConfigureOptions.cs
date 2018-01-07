using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Vault.Shared.Authentication.Pocket
{
    /// <summary>
    /// Used to setup defaults for all <see cref="PocketOptions"/>.
    /// </summary>
    public class PocketPostConfigureOptions : IPostConfigureOptions<PocketOptions>
    {
        private readonly IDataProtectionProvider _dp;

        public PocketPostConfigureOptions(IDataProtectionProvider dataProtection)
        {
            _dp = dataProtection;
        }

        /// <summary>
        /// Invoked to post configure a TOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void PostConfigure(string name, PocketOptions options)
        {
            options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;

            if (options.StateDataFormat == null)
            {
                var dataProtector = options.DataProtectionProvider.CreateProtector(
                    typeof(PocketAuthenticationHandler).FullName, name, "v1");
                options.StateDataFormat = new SecureDataFormat<RequestToken>(
                    new RequestTokenSerializer(),
                    dataProtector);
            }

            if (options.Backchannel == null)
            {
                options.Backchannel =
                    new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler())
                    {
                        Timeout = options.BackchannelTimeout,
                        MaxResponseContentBufferSize = 1024 * 1024 * 10 // 10 MB
                    };
                options.Backchannel.DefaultRequestHeaders.Accept.ParseAdd("*/*");
                options.Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core Twitter handler");
                options.Backchannel.DefaultRequestHeaders.ExpectContinue = false;
            }
        }
    }
}
