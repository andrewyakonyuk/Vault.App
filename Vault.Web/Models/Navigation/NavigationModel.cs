using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Web.Models.Navigation
{
    public class NavigationModel
    {
        public NavigationModel()
        {
            Boards = new List<NavigationItem>();
        }

        public List<NavigationItem> Boards { get; set; }
    }
}