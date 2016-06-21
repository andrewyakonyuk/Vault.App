using NEventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared.Events;

namespace Vault.Shared.NEventStore
{
    public class EventsRebuilder : IEventsRebuilder
    {
        private readonly IStoreEvents _store;
        private readonly IEventPublisher _publisher;

        public EventsRebuilder(IStoreEvents store, IEventPublisher publisher)
        {
            _store = store;
            _publisher = publisher;
        }

        public async Task RebuildAsync()
        {
            try
            {
                var commits = _store.Advanced.GetFrom(null).ToArray();

                foreach (var commit in commits)
                {
                    var evts = commit.Events
                        .Where(x => x.Body is IEvent)
                        .Select(evt => (IEvent)evt.Body);

                    await _publisher.PublishAsync(evts);
                }
            }
            catch (Exception ex)
            {
                var a = 5;
            }
        }
    }
}