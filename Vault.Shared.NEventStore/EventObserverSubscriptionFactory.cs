using NEventStore;
using NEventStore.Client;
using System;
using System.Collections.Generic;

namespace Vault.Shared.NEventStore
{
    public class EventObserverSubscriptionFactory
    {
        private readonly IStoreEvents _eventStore;
        private readonly ICheckpointRepository _checkpointRepo;
        private readonly IEnumerable<IObserver<ICommit>> _commitObservers;

        public EventObserverSubscriptionFactory(IStoreEvents eventStore,
            ICheckpointRepository checkpointRepo,
            IEnumerable<IObserver<ICommit>> commitObservers)
        {
            if (eventStore == null)
                throw new ArgumentNullException(nameof(eventStore));
            if (checkpointRepo == null)
                throw new ArgumentNullException(nameof(checkpointRepo));
            if (commitObservers == null)
                throw new ArgumentNullException(nameof(commitObservers));

            this._checkpointRepo = checkpointRepo;
            this._eventStore = eventStore;
            this._commitObservers = commitObservers;
        }

        public IObserveCommits Construct()
        {
            var pollingClient = new PollingClient(_eventStore.Advanced);
            var checkpoint = _checkpointRepo.LoadCheckpoint();
            IObserveCommits subscription = pollingClient.ObserveFrom(checkpoint);

            foreach (var commitObserver in _commitObservers)
            {
                subscription.Subscribe(commitObserver);
            }

            return subscription;
        }
    }
}