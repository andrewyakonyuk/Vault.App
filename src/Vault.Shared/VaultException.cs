using System;

namespace Vault.Shared
{
    /// <summary>
    /// Represent a base application exception.
    /// </summary>
    [Serializable]
    public class VaultException : Exception
    {
        public VaultException()
        {
        }

        public VaultException(string message) : base(message)
        {
        }

        public VaultException(string message, Exception inner) : base(message, inner)
        {
        }

        protected VaultException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}