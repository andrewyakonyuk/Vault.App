using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.Extensions.OptionsModel;
using System;

namespace Vault.Framework.Mvc.Routing
{
    public class DefaultUrlHelper : UrlHelper
    {
        private readonly UrlOptions _options;

        public DefaultUrlHelper(IActionContextAccessor contextAccessor,
            IActionSelector actionSelector,
            IOptions<UrlOptions> options)
            : base(contextAccessor, actionSelector)
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