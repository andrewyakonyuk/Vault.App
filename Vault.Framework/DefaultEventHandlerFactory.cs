using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Vault.Shared.Events;

namespace Vault.Framework
{
    public class DefaultEventHandlerFactory : IEventHandlerFactory
    {
        readonly IServiceProvider _serviceProvider;

        public DefaultEventHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<IHandle> GetHandlers<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            return (IEnumerable<IHandle>)_serviceProvider.GetServices(typeof(IHandle<>).MakeGenericType(@event.GetType())) ?? Enumerable.Empty<IHandle>();
        }
    }
}