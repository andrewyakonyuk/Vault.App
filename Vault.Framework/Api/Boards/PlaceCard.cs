using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Framework.Api.Boards
{
    public class PlaceCard : Card
    {
        public decimal Elevation { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Thumbnail { get; set; }
    }
}