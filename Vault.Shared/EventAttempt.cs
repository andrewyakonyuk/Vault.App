using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared
{
    public interface ICorrelationMessage
    {
        string CorrelationId { get; set; }
    }

    public abstract class EventAttempt : ICorrelationMessage
    {
        /// <summary>
        /// Gets the correlation id.
        /// </summary>
        public string CorrelationId { get; set; }

        public object Body { get; protected set; }

        public static EventAttempt<T> Create<T>(T body)
        {
            return new EventAttempt<T>(body);
        }
    }

    [Serializable]
    public class EventAttempt<T> : EventAttempt
    {
        public EventAttempt()
        {
        }

        public EventAttempt(T body)
        {
            Body = body;
        }

        /// <summary>
        /// Gets the body.
        /// </summary>
        public new T Body
        {
            get { return (T)base.Body; }
            set { base.Body = value; }
        }
    }
}