using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Connectors
{
    public class UserInfo
    {
        readonly string _accessCode;

        public UserInfo(string accessCode)
        {
            if (string.IsNullOrEmpty(accessCode))
                throw new ArgumentNullException(nameof(accessCode));

            _accessCode = accessCode;
        }

        public string AccessCode { get { return _accessCode; } }
    }
}
