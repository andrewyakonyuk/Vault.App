using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;
using Vault.Shared;
using Vault.Activity.Sinks;
using Vault.Activity.Utility;
using Vault.Activity.Persistence;
using Vault.Shared.Search;
using Vault.Shared.Search.Parsing;

namespace Vault.Activity.Client
{
    public interface IActivityFeed
    {
        Task PushActivityAsync(ActivityEventAttempt activity);

        Task<IReadOnlyCollection<CommitedActivityEvent>> ReadEventsAsync(long checkpointToken, int maxCount);

        Task<IReadOnlyCollection<CommitedActivityEvent>> SearchEventsAsync(string query, long checkpointToken, int maxCount);
    }

    [Serializable]
    public class ActivityFeed : IActivityFeed
    {
        readonly ISink<UncommitedActivityEvent> _sink;
        readonly IAppendOnlyStore _appendOnlyStore;
        readonly string _bucket;
        readonly Guid _streamId;
        readonly IClock _clock;
        readonly ISearchProvider _searchProvider;
        readonly ISearchQueryParser _queryParser;

        public ActivityFeed(
            string bucket,
            Guid streamId,
            ISink<UncommitedActivityEvent> sink,
            IAppendOnlyStore appendOnlyStore,
            ISearchProvider searchProvider,
            ISearchQueryParser queryParser,
            IClock clock)
        {
            if (string.IsNullOrEmpty(bucket))
                throw new ArgumentNullException(nameof(bucket));
            if (sink == null)
                throw new ArgumentNullException(nameof(sink));
            if (appendOnlyStore == null)
                throw new ArgumentNullException(nameof(appendOnlyStore));
            if (searchProvider == null)
                throw new ArgumentException(nameof(searchProvider));
            if (queryParser == null)
                throw new ArgumentNullException(nameof(queryParser));
            if (clock == null)
                throw new ArgumentNullException(nameof(clock));

            _sink = sink;
            _appendOnlyStore = appendOnlyStore;
            _searchProvider = searchProvider;
            _queryParser = queryParser;
            _bucket = bucket;
            _streamId = streamId;
            _clock = clock;
        }

        public async Task<IReadOnlyCollection<CommitedActivityEvent>> SearchEventsAsync(string query, long checkpointToken, int maxCount)
        {
            if (maxCount <= 0)
                return new List<CommitedActivityEvent>();

            var parsedQuery = _queryParser.Parse(query);
            var searchCriteria = parsedQuery.AsCriteria();

            maxCount = Math.Min(maxCount, 100);

            var searchRequest = new SearchRequest
            {
                Count = maxCount,
                Criteria = searchCriteria.ToList(),
                IndexName = _bucket,
                OwnerId = _streamId.ToString("N")
            };
            //todo: include checkpointToken and bucket vars into search request

            var searchResults = _searchProvider.Search(searchRequest);
            var checkpointTokens = new List<long>(searchResults.Count);

            foreach (dynamic item in searchResults)
            {
                checkpointTokens.Add(item.CheckpointToken);
            }

            return await _appendOnlyStore.ReadRecordsAsync(checkpointTokens);
        }

        public async Task<IReadOnlyCollection<CommitedActivityEvent>> ReadEventsAsync(long checkpointToken, int maxCount)
        {
            if (maxCount <= 0)
                return new List<CommitedActivityEvent>();

            maxCount = Math.Min(maxCount, 100);

            return await _appendOnlyStore.ReadRecordsAsync(_streamId, _bucket, checkpointToken, maxCount);
        }

        public Task PushActivityAsync(ActivityEventAttempt activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));
            if (string.IsNullOrEmpty(activity.Id))
                throw new ArgumentException("Id is required", nameof(activity.Id));

            var uncommitedEvent = new UncommitedActivityEvent
            {
                StreamId = _streamId,
                Bucket = _bucket,
                Actor = activity.Actor,
                Content = activity.Content,
                Id = activity.Id,
                MetaBag = activity.MetaBag,
                Provider = activity.Provider,
                Published = activity.Published,
                Target = activity.Target,
                Title = activity.Title,
                Uri = activity.Uri,
                Verb = activity.Verb
            };

            if (string.IsNullOrEmpty(uncommitedEvent.Verb))
                uncommitedEvent.Verb = ActivityVerbs.Post;

            if (uncommitedEvent.Published == default(DateTimeOffset))
                uncommitedEvent.Published = _clock.OffsetUtcNow;

            if (string.IsNullOrEmpty(uncommitedEvent.Actor))
                uncommitedEvent.Actor = _streamId.ToString("N");

            _sink.Emit(uncommitedEvent);
            return TaskDone.Done;
        }
    }
}