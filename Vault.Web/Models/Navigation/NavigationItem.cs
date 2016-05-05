using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Web.Models.Navigation
{
    public class NavigationItem
    {
        public string Text { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }
        public int Position { get; set; }
    }
}