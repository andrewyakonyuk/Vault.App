using System;

namespace Vault.Activity.Services.Connectors
{
    public class UserInfo
    {
        readonly string _accessCode;
        readonly string _id;

        public UserInfo(string accessCode, string id)
        {
            if (string.IsNullOrEmpty(accessCode))
                throw new ArgumentNullException(nameof(accessCode));
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            _accessCode = accessCode;
            _id = id;
        }

        public string Id { get { return _id; } }

        public string AccessCode { get { return _accessCode; } }
    }
}