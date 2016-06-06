using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Domain.Models
{
    /// <summary>
    /// An entry point, within some Web-based protocol.
    /// </summary>
    public class EntryPoint
    {
        /// <summary>
        /// The high level platform(s) where the Action can be performed for the given URL. 
        /// To specify a specific application or operating system instance, use actionApplication
        /// </summary>
        public string ActionPlatform { get; set; }

        /// <summary>
        /// The supported content type(s) for an EntryPoint response.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The supported encoding type(s) for an EntryPoint request.
        /// </summary>
        public string EncodingType { get; set; }

        /// <summary>
        /// An HTTP method that specifies the appropriate HTTP method for 
        /// a request to an HTTP EntryPoint. Values are capitalized strings as used in HTTP. 
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// An url template (RFC6570) that will be used to construct the target of the execution of the action
        /// </summary>
        public string UrlTemplate { get; set; }
    }
}
