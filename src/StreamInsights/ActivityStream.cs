using StreamInsights.Abstractions;
using StreamInsights.Persistance;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace StreamInsights
{
    public class ActivityStream : IActivityStream
    {
        readonly ITargetBlock<UncommitedActivity> _target;
        readonly IAppendOnlyActivityStore _appendOnlyStore;
        readonly string _bucket;
        readonly string _streamId;
        readonly IClock _clock;

        private static readonly IReadOnlyCollection<CommitedActivity> EmptyCommitedList = new List<CommitedActivity>(0).AsReadOnly();
        private static readonly Task<IReadOnlyCollection<CommitedActivity>> EmptyCommitedListTask = Task.FromResult(EmptyCommitedList);

        public ActivityStream(
            string bucket,
            string streamId,
            ITargetBlock<UncommitedActivity> target,
            IAppendOnlyActivityStore appendOnlyStore,
            IClock clock)
        {
            if (string.IsNullOrEmpty(bucket))
                throw new ArgumentNullException(nameof(bucket));
            if (string.IsNullOrEmpty(streamId))
                throw new ArgumentNullException(nameof(streamId));
            
            _bucket = bucket;
            _streamId = streamId;
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _appendOnlyStore = appendOnlyStore ?? throw new ArgumentNullException(nameof(appendOnlyStore));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public Task<IReadOnlyCollection<CommitedActivity>> ReadActivityAsync(long checkpointToken, int maxCount, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();
            if (maxCount <= 0)
                return EmptyCommitedListTask;

            maxCount = Math.Min(maxCount, 500);

            return _appendOnlyStore.ReadAsync(_streamId, _bucket, checkpointToken, maxCount, token);
        }

        public Task PushActivityAsync(Activity activity, CancellationToken token = default(CancellationToken))
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));
            token.ThrowIfCancellationRequested();

            ValidateActivity(activity);

            var uncommitedEvent = new UncommitedActivity(activity)
            {
                StreamId = _streamId,
                Bucket = _bucket
            };

            if (string.IsNullOrEmpty(activity.Id))
                uncommitedEvent.Id = $"_:0-{Guid.NewGuid():N}";

            if (!uncommitedEvent.Type.HasValue)
                uncommitedEvent.Type = "Post";

            if (!uncommitedEvent.Published.HasValue)
                uncommitedEvent.Published = _clock.UtcNowOffset;

            return _target.SendAsync(uncommitedEvent, token);
        }

        private void ValidateActivity(Activity activity)
        {
        }
    }
}
