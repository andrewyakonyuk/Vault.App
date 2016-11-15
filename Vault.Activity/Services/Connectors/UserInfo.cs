using System;

namespace Vault.Activity.Services.Connectors
{
    public class UserInfo
    {
        readonly string _accessCode;
        readonly Guid _id;

        public UserInfo(string accessCode, Guid id)
        {
            if (string.IsNullOrEmpty(accessCode))
                throw new ArgumentNullException(nameof(accessCode));
            if (id == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(id));

            _accessCode = accessCode;
            _id = id;
        }

        public Guid Id { get { return _id; } }

        public string AccessCode { get { return _accessCode; } }
    }
}