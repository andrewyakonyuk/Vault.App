using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.WebApp.Infrastructure.Spouts
{
    public class SpoutOptions
    {
        public SpoutOptions()
        {
            Services = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, Type> Services { get; }
    }
}
