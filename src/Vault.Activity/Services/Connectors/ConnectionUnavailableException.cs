using System;
using System.Runtime.Serialization;

namespace Vault.Activity.Services.Connectors
{
    [Serializable]
    public class ConnectionUnavailableException : ApplicationException
    {
        public ConnectionUnavailableException(string api)
            : base(api + " api is unavailable. Try later!")
        { }

        protected ConnectionUnavailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}