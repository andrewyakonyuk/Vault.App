using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace Vault.WebHost.Models.Navigation
{
    public class NavigationItem
    {
        public string Text { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }
        public int Position { get; set; }
    }
}