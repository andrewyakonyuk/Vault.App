using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Activity.Sinks
{
    /// <summary>
    /// A destination for messages.
    /// </summary>
    public interface ISink<T>
    {
        /// <summary>
        /// Emit the provided message to the sink.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Emit(T message);
    }
}
