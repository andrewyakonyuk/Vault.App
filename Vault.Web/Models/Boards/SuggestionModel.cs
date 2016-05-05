using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Framework.Api.Boards;

namespace Vault.Web.Models.Boards
{
    public class SuggestionModel
    {
        public string Value { get; set; }
        public string Type { get; set; }
        public Card Card { get; set; }
    }
}