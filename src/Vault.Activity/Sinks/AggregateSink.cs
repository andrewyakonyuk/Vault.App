using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Activity.Sinks
{
    public class AggregateSink<T> : ISink<T>
    {
        readonly ISink<T>[] _sinks;

        public AggregateSink(IEnumerable<ISink<T>> sinks)
        {
            if (sinks == null) throw new ArgumentNullException(nameof(sinks));
            _sinks = sinks.ToArray();
        }

        public void Emit(T message)
        {
            List<Exception> exceptions = null;
            foreach (var sink in _sinks)
            {
                try
                {
                    sink.Emit(message);
                }
                catch (Exception ex)
                {
                    exceptions = exceptions ?? new List<Exception>();
                    exceptions.Add(ex);
                }
            }

            if (exceptions != null)
                throw new AggregateException("Failed to emit a log event.", exceptions);
        }
    }
}
