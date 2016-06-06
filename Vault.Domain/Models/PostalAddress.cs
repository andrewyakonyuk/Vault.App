using Vault.Shared.Domain;

namespace Vault.Domain.Models
{
    public class PostalAddress : IEntityComponent
    {
        /// <summary>
        /// The country. For example, USA.
        /// You can also provide the two-letter ISO 3166-1 alpha-2 country code.
        /// </summary>
        public virtual string Country { get; set; }

        /// <summary>
        /// The locality. For example, Mountain View.
        /// </summary>
        public virtual string Locality { get; set; }

        /// <summary>
        /// The region. For example, CA.
        /// </summary>
        public virtual string Region { get; set; }

        /// <summary>
        /// The postal code. For example, 94043.
        /// </summary>
        public virtual string PostalCode { get; set; }

        /// <summary>
        /// The street address. For example, 1600 Amphitheatre Pkwy.
        /// </summary>
        public virtual string StreetAddress { get; set; }
    }
}