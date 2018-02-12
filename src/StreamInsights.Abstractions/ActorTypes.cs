using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamInsights.Abstractions
{
    public class ActorTypes
    {
        /// <summary>
        /// Describes a software application.
        /// </summary>
        public const string Application = "application";

        /// <summary>
        /// Represents a formal or informal collective of Actors.
        /// </summary>
        public const string Group = "group";

        /// <summary>
        /// Represents an organization.
        /// </summary>
        public const string Organization = "organization";

        /// <summary>
        /// Represents an individual person
        /// </summary>
        public const string Person = "person";

        /// <summary>
        /// Represents a service of any kind.
        /// </summary>
        public const string Service = "service";
    }
}
