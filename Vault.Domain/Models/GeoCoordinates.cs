using Vault.Shared.Domain;

namespace Vault.Domain.Models
{
    public class GeoCoordinates : IEntityComponent
    {
        /// <summary>
        /// The elevation of a location
        /// </summary>
        public virtual decimal Elevation { get; set; }

        /// <summary>
        /// The latitude of a location. For example 37.42242
        /// </summary>
        public virtual decimal Latitude { get; set; }

        /// <summary>
        /// The longitude of a location. For example -122.08585
        /// </summary>
        public virtual decimal Longitude { get; set; }
    }
}