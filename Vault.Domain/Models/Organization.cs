using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Domain.Models
{
    public abstract class Organization : Thing
    {
        /// <summary>
        /// Physical address of the item.
        /// </summary>
        public virtual PostalAddress Address { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        public virtual string Email { get; set; }

        public virtual string FaxNo { get; set; }

        public virtual DateTime FoundingDate { get; set; }

        public virtual Place FoundingLocation { get; set; }

        public virtual string LegalName { get; set; }
    }
}