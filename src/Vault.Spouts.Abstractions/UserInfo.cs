using System;

namespace Vault.Spouts.Abstractions
{
    public class UserInfo
    {
        public UserInfo(string accessCode, string id)
        {
            if (string.IsNullOrEmpty(accessCode))
                throw new ArgumentNullException(nameof(accessCode));
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            AccessCode = accessCode;
            Id = id;
        }

        public string Id { get; }

        public string AccessCode { get; }
    }
}
