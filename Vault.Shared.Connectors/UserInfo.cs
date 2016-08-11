using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Connectors
{
    public class UserInfo
    {
        readonly string _accessCode;
        readonly int _id;

        public UserInfo(string accessCode, int id)
        {
            if (string.IsNullOrEmpty(accessCode))
                throw new ArgumentNullException(nameof(accessCode));

            _accessCode = accessCode;
            _id = id;
        }

        public string AccessCode { get { return _accessCode; } }

        public int Id { get { return _id; } }
    }
}