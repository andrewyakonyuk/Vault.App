using Vault.Shared.Domain;

namespace Vault.Domain.Models
{
    public class Place : Thing, IEntity
    {
        /// <summary>
        /// Physical address of the item.
        /// </summary>
        public virtual PostalAddress Address { get; set; }

        /// <summary>
        /// The geo coordinates of the place.
        /// </summary>
        public virtual GeoCoordinates Geo { get; set; }
    }
}