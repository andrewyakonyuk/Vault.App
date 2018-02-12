using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamInsights.Abstractions
{
    public class LinkTypes
    {
        public const string Link = "link";

        /// <summary>
        /// A specialized Link that represents an @mention.
        /// </summary>
        public const string Mention = "mention";
    }
}
