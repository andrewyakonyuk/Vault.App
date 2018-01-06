using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vault.Activity.Sinks;

namespace Vault.Activity.Indexes
{
    public class IndexBatchingAdapter<TDocument> : IPeriodicBatchingAdapter<TDocument>
    {
        readonly IIndexStoreAccessor _indexStoreAccessor;
        readonly IEnumerable<AbstractIndexCreationTask<TDocument>> _indexCreationTasks;

        public IndexBatchingAdapter(
            IIndexStoreAccessor indexStoreAccessor,
            IEnumerable<AbstractIndexCreationTask<TDocument>> indexCreationTasks)
        {
            _indexCreationTasks = indexCreationTasks;
            _indexStoreAccessor = indexStoreAccessor;
        }

        public int BatchSizeLimit => 100;

        public TimeSpan Period => TimeSpan.FromSeconds(3);

        public int? QueueLimit => 1000;

        public bool CanInclude(TDocument message)
        {
            return true;
        }

        public async Task EmitBatchAsync(IEnumerable<TDocument> messages)
        {
            foreach (var indexTask in _indexCreationTasks)
            {
                var indexStore = _indexStoreAccessor.NewIndexStore(indexTask);
                await indexTask.ExecuteAsync(messages, indexStore);
            }
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
