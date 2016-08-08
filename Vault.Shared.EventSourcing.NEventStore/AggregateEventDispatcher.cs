using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vault.Shared.EventSourcing.NEventStore
{
    public class AggregateEventDispatcher
    {
        private readonly IDictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        private readonly bool _throwOnApplyNotFound;

        private IEventProvider _registered;

        public AggregateEventDispatcher(bool throwOnApplyNotFound, IEventProvider aggregate)
        {
            _throwOnApplyNotFound = throwOnApplyNotFound;
            this.Register(aggregate);
        }

        public virtual void Register(IEventProvider eventProvider)
        {
            if (eventProvider == null)
            {
                throw new ArgumentNullException("aggregate");
            }
            this._registered = eventProvider;
            var applyMethods = eventProvider.GetType()
                               .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                               .Where(m => m.Name == "Apply" && m.GetParameters().Length == 1 && m.ReturnParameter.ParameterType == typeof(void))
                               .Select(m => new
                               {
                                   Method = m,
                                   MessageType = m.GetParameters().Single<ParameterInfo>().ParameterType
                               });
            foreach (var apply in applyMethods)
            {
                MethodInfo applyMethod = apply.Method;
                this._handlers.Add(apply.MessageType, delegate (object m)
                {
                    applyMethod.Invoke(eventProvider, new object[]
                    {
                        m
                    });
                });
            }
        }

        public virtual void Dispatch(IEvent eventMessage)
        {
            if (eventMessage == null)
            {
                throw new ArgumentNullException("eventMessage");
            }
            Action<object> handler;
            if (this._handlers.TryGetValue(eventMessage.GetType(), out handler))
            {
                handler(eventMessage);
                return;
            }
            if (this._throwOnApplyNotFound)
            {
                throw new InvalidOperationException();
            }
        }

        public virtual void Register<T>(Action<T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.Register(typeof(T), delegate (object @event)
            {
                handler((T)((object)@event));
            });
        }

        private void Register(Type messageType, Action<object> handler)
        {
            this._handlers[messageType] = handler;
        }
    }
}