using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vault.Activity.Sinks;

namespace Vault.Activity.Persistence
{
    public class AppendOnlyStoreBatchingAdapter : IPeriodicBatchingAdapter<UncommitedActivityEvent>
    {
        readonly IAppendOnlyStore _store;

        public AppendOnlyStoreBatchingAdapter(IAppendOnlyStore store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            _store = store;
        }

        public int BatchSizeLimit => 100;

        public TimeSpan Period => TimeSpan.FromSeconds(3);

        public int? QueueLimit => null;

        public bool CanInclude(UncommitedActivityEvent message)
        {
            return true;
        }

        public async Task EmitBatchAsync(IEnumerable<UncommitedActivityEvent> messages)
        {
            await _store.AppendAsync(messages);
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.FromResult(true);
        }

        public void Dispose()
        {

        }
    }
}
