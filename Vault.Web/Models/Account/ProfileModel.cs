using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.WebHost.Models.Account
{
    public class ProfileModel
    {
        public ProfileModel()
        {
            Logins = new List<ExternalLoginModel>();
        }

        public string Email { get; set; }

        public string Username { get; set; }

        public List<ExternalLoginModel> Logins { get; set; }
    }
}