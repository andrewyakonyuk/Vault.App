using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Activity.Sinks;
using Vault.Activity.Utility;
using Vault.Shared;
using Vault.Shared.Search;

namespace Vault.Activity.Indexes
{
    public interface IIndexExecutor : IGrainWithGuidKey
    {
    }

    [ImplicitStreamSubscription("timeline")]
    public class IndexExecutor : Grain, IIndexExecutor
    {
        private ISink<CommitedActivityEvent> _sink;

        public IndexExecutor(ISink<CommitedActivityEvent> sink)
        {
            _sink = sink;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            var streamProvider = GetStreamProvider("EventStream");
            var consumer = streamProvider.GetStream<CommitedActivityEvent>(this.GetPrimaryKey(), "timeline");
            await consumer.SubscribeAsync((@event, token) => NewActivityAsync(@event, token));
        }

        public Task NewActivityAsync(CommitedActivityEvent @event, StreamSequenceToken token)
        {
            _sink.Emit(@event);
            return TaskDone.Done;
        }
    }
}
