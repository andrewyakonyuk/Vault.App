using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Vault.Activity.Indexes;
using Vault.Activity.Persistence;
using Vault.Shared.Activity;

namespace Vault.Activity
{
    public class StreamFlowBuilder
    {
        readonly IIndexStoreAccessor _indexStoreAccessor;
        readonly IEnumerable<AbstractIndexCreationTask<CommitedActivityEvent>> _indexCreationTasks;
        readonly IAppendOnlyStore _appendStore;

        public StreamFlowBuilder(
            IIndexStoreAccessor indexStoreAccessor,
            IEnumerable<AbstractIndexCreationTask<CommitedActivityEvent>> indexCreationTasks,
            IAppendOnlyStore appendStore)
        {
            _indexCreationTasks = indexCreationTasks;
            _indexStoreAccessor = indexStoreAccessor;
            _appendStore = appendStore;
        }

        public Task Configure(CancellationToken token)
        {
            var commitedEventsBuffer = new BufferBlock<CommitedActivityEvent>(new DataflowBlockOptions
            {
                BoundedCapacity = 100,
                CancellationToken = token
            });
            var broadcaster = new BroadcastBlock<CommitedActivityEvent>(e => e, new DataflowBlockOptions
            {
                BoundedCapacity = 100,
                CancellationToken = token
            });
            var batcher = new BatchBlock<CommitedActivityEvent>(25, new GroupingDataflowBlockOptions
            {
                BoundedCapacity = 100,
                Greedy = true,
                CancellationToken = token
            });
            var indexProcessor = new ActionBlock<CommitedActivityEvent[]>(async commitedEvents =>
            {
                foreach (var indexTask in _indexCreationTasks)
                {
                    var indexStore = _indexStoreAccessor.NewIndexStore(indexTask);
                    await indexTask.ExecuteAsync(commitedEvents, indexStore);
                }
            }, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 100,
                CancellationToken = token
            });
            
            commitedEventsBuffer.LinkTo(broadcaster, new DataflowLinkOptions { PropagateCompletion = true });
            broadcaster.LinkTo(batcher, new DataflowLinkOptions { PropagateCompletion = true });
            batcher.LinkTo(indexProcessor, new DataflowLinkOptions { PropagateCompletion = true });

            var checkpoint = 0L;
            var pollingCancellationSource = new CancellationTokenSource();
            var pollingCancellation = pollingCancellationSource.Token;
            var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(pollingCancellation, token);
            var pollingTask = new Task(async () =>
            {
                TimeSpan interval = TimeSpan.Zero;
                TimeSpan waitAfterSuccessInterval = TimeSpan.Zero;
                TimeSpan waitAfterErrorInterval = TimeSpan.FromSeconds(10);
                TimeSpan waitAfterEmptyResultInterval = TimeSpan.FromSeconds(10);
                while (!pollingCancellation.WaitHandle.WaitOne(interval))
                {
                    try
                    {
                        var result = await _appendStore.ReadRecordsAsync(checkpoint, 100);
                        if (result.Count == 0)
                            interval = waitAfterEmptyResultInterval;
                        else
                        {
                            foreach (var item in result)
                            {
                                await commitedEventsBuffer.SendAsync(item, token);
                                checkpoint = item.CheckpointToken;
                            }
                            interval = waitAfterSuccessInterval;

                            if (result.Count < 100)
                                batcher.TriggerBatch();
                        }

                        // Occasionally check the cancellation state.
                        if (linkedCancellation.IsCancellationRequested)
                        {
                            commitedEventsBuffer.Complete();
                            break;
                        }
                    }
                    catch (Exception caught)
                    {
                        //check whenever exception is transient
                        if (caught is TimeoutException 
                            || caught is OperationCanceledException
                            || caught is TaskCanceledException)
                        {
                            // Log the exception and try one more
                            interval = waitAfterErrorInterval;
                        }
                        else throw;
                    }
                }
            }, pollingCancellationSource.Token, TaskCreationOptions.LongRunning);
            pollingTask.Start();

            return indexProcessor.Completion;
        }
    }
    
}
