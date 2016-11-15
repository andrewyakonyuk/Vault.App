using System;
using NEventStore;
using NEventStore.Dispatcher;
using Orleans;
using Orleans.Concurrency;
using Vault.Activity.Events;
using Vault.Activity.Services.ActivityLog;

namespace Vault.Activity.Host
{
    public class DistributedDispatchCommits : IDispatchCommits
    {
        readonly IGrainFactory _grainFactory;

        public DistributedDispatchCommits(IGrainFactory grainFactory)
        {
            if (grainFactory == null)
                throw new ArgumentNullException(nameof(grainFactory));

            _grainFactory = grainFactory;
        }

        public async void Dispatch(ICommit commit)
        {
            foreach (var @event in commit.Events)
            {
                var activityEvent = @event.Body as ActivityEventBase;
                if (activityEvent != null)
                {
                    var notifier = _grainFactory.GetGrain<IPushActivityNotifier<ActivityEventBase>>(activityEvent.ItemKey.OwnerId);
                    await notifier.NewActivityAsync(activityEvent.AsImmutable());
                    await notifier.CompleteAsync();
                }
            }
        }

        public void Dispose()
        {
        }
    }
}