using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace StreamInsights
{
    public class PartitionedActionBlock<TInput> : ITargetBlock<TInput>
    {
        private readonly ITargetBlock<TInput> _distributor;
        private readonly ActionBlock<PartitionEntry<TInput>>[] _workers;

        public Task Completion
        {
            get { return Task.WhenAll(_workers.Select(x => x.Completion)); }
        }

        public PartitionedActionBlock(Func<PartitionEntry<TInput>, Task> action, Func<TInput, int> partitioner)
            : this(action, partitioner, new ExecutionDataflowBlockOptions())
        {
        }


        public PartitionedActionBlock(
            Func<PartitionEntry<TInput>, Task> action,
            Func<TInput, int> partitioner,
            ExecutionDataflowBlockOptions dataflowBlockOptions)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (dataflowBlockOptions == null)
                throw new ArgumentNullException(nameof(dataflowBlockOptions));

            dataflowBlockOptions.MaxDegreeOfParallelism = Math.Max(dataflowBlockOptions.MaxDegreeOfParallelism, 1);
            _workers = new ActionBlock<PartitionEntry<TInput>>[dataflowBlockOptions.MaxDegreeOfParallelism];

            for (var i = 0; i < dataflowBlockOptions.MaxDegreeOfParallelism; i++)
            {
                var workerOption = new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = dataflowBlockOptions.BoundedCapacity,
                    CancellationToken = dataflowBlockOptions.CancellationToken,
                    NameFormat = dataflowBlockOptions.NameFormat,
                    SingleProducerConstrained = dataflowBlockOptions.SingleProducerConstrained,
                    TaskScheduler = dataflowBlockOptions.TaskScheduler,

                    MaxDegreeOfParallelism = 1,
                    MaxMessagesPerTask = 1
                };

                _workers[i] = new ActionBlock<PartitionEntry<TInput>>(action, workerOption);
            }

            var distributorOption = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1,
                MaxMessagesPerTask = 1,
                BoundedCapacity = 1
            };

            _distributor = new ActionBlock<TInput>(x =>
            {
                var partition = Math.Abs(partitioner(x)) % _workers.Length;

                var entry = new PartitionEntry<TInput>(x, partition);
                return _workers[partition].SendAsync(entry);
            }, distributorOption);

            _distributor.Completion.ContinueWith(x =>
            {
                foreach (var worker in _workers)
                {
                    worker.Complete();
                }
            });
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept)
        {
            return _distributor.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

        public void Complete()
        {
            _distributor.Complete();
        }

        public void Fault(Exception exception)
        {
            _distributor.Fault(exception);
        }
    }

    public class PartitionEntry<T>
    {
        public readonly T Source;
        public readonly int PartitionId;

        public PartitionEntry(T source, int partitionId)
        {
            Source = source;
            PartitionId = partitionId;
        }
    }
}
