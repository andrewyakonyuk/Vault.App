using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using System;

namespace Vault.Framework.Mvc.Routing
{
    public class DefaultUrlHelper : UrlHelper
    {
        private readonly UrlOptions _options;

        public DefaultUrlHelper(
            ActionContext actionContext,
            IOptions<UrlOptions> options)
            : base(actionContext)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Depending on config data, generates an absolute url pointing to a CDN server
        /// or falls back to the default behavior
        /// </summary>
        /// <param name="contentPath">The virtual path of the content.</param>
        /// <returns>The absolute url.</returns>
        public override string Content(string contentPath)
        {
            if (_options.ServeCDNContent
                && contentPath.StartsWith("~/", StringComparison.Ordinal))
            {
                var segment = new PathString(contentPath.Substring(1));

                return _options.CDNServerBaseUrl + segment;
            }

            return base.Content(contentPath);
        }
    }
}