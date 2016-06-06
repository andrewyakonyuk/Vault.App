using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Connectors
{
    [Serializable]
    public class PullConnectionException : Exception
    {
        public PullConnectionException() { }
        public PullConnectionException(string message) : base(message) { }
        public PullConnectionException(string message, Exception inner) : base(message, inner) { }
        protected PullConnectionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
