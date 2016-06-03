using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Vault.Shared.Events;

namespace Vault.Framework
{
    public class DefaultEventHandlerFactory : IEventHandlerFactory
    {
        readonly IServiceProvider _serviceProvider;
        readonly ILogger<DefaultEventHandlerFactory> _logger;

        public DefaultEventHandlerFactory(
            IServiceProvider serviceProvider,
            ILogger<DefaultEventHandlerFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public IEnumerable<IHandle> GetHandlers<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            try
            {
                return (IEnumerable<IHandle>)_serviceProvider.GetServices(typeof(IHandle<>).MakeGenericType(@event.GetType())) ?? Enumerable.Empty<IHandle>();
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex.Message);
                throw;
            }
        }
    }
}