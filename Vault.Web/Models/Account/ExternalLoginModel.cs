using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.WebHost.Models.Account
{
    public class ExternalLoginModel
    {
        public string AuthenticationScheme { get; set; }
        public string DisplayName { get; set; }
        public bool HasLogin { get; set; }
        public bool IsValid { get; set; }
        public int? ExternalLoginId { get; set; }
    }
}