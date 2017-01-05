using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;
using Vault.Shared;

namespace Vault.Activity
{
    public interface IActivityFeed : IGrainWithGuidCompoundKey
    {
        Task NewActivityAsync(IList<ActivityEvent> activity);
    }

    public class ActivityFeed : Grain, IActivityFeed
    {
        IAsyncStream<ActivityEvent> _consumer;

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            var streamProvider = GetStreamProvider("EventStream");
            string keyExt = null;
            _consumer = streamProvider.GetStream<ActivityEvent>(this.GetPrimaryKey(out keyExt), "activity-log");
        }

        public async Task NewActivityAsync(IList<ActivityEvent> activities)
        {
            await _consumer.OnNextBatchAsync(activities);
        }
    }
}