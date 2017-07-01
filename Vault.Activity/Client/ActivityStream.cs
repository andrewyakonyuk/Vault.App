using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Activity.Sinks;
using Vault.Activity.Utility;
using Vault.Activity.Persistence;
using Vault.Shared.Search;
using Vault.Shared.Search.Parsing;
using Vault.Activity.Indexes;
using Vault.Shared.Activity;

namespace Vault.Activity.Client
{
    [Serializable]
    public class ActivityStream : IActivityStream
    {
        readonly ISink<UncommitedActivityEvent> _sink;
        readonly IAppendOnlyStore _appendOnlyStore;
        readonly string _bucket;
        readonly string _streamId;
        readonly IClock _clock;
        readonly IIndexStoreAccessor _indexAccessor;
        readonly ISearchQueryParser _queryParser;

        public ActivityStream(
            string bucket,
            string streamId,
            ISink<UncommitedActivityEvent> sink,
            IAppendOnlyStore appendOnlyStore,
            IIndexStoreAccessor indexAccessor,
            ISearchQueryParser queryParser,
            IClock clock)
        {
            if (string.IsNullOrEmpty(bucket))
                throw new ArgumentNullException(nameof(bucket));
            if (sink == null)
                throw new ArgumentNullException(nameof(sink));
            if (appendOnlyStore == null)
                throw new ArgumentNullException(nameof(appendOnlyStore));
            if (indexAccessor == null)
                throw new ArgumentException(nameof(indexAccessor));
            if (queryParser == null)
                throw new ArgumentNullException(nameof(queryParser));
            if (clock == null)
                throw new ArgumentNullException(nameof(clock));

            _sink = sink;
            _appendOnlyStore = appendOnlyStore;
            _indexAccessor = indexAccessor;
            _queryParser = queryParser;
            _bucket = bucket;
            _streamId = streamId;
            _clock = clock;
        }

        public async Task<IReadOnlyCollection<CommitedActivityEvent>> ReadEventsAsync(string query, long checkpointToken, int maxCount)
        {
            if (maxCount <= 0)
                return new List<CommitedActivityEvent>();

            maxCount = Math.Min(maxCount, 100);

            if (!string.IsNullOrEmpty(query))
                return await SearchEventsAsync(query, checkpointToken, maxCount);

            return await _appendOnlyStore.ReadRecordsAsync(_streamId, _bucket, checkpointToken, maxCount);
        }

        public Task PushActivityAsync(ActivityEventAttempt activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));
            if (string.IsNullOrEmpty(activity.Id))
                throw new ArgumentException("Id must be provided for an activity", nameof(activity.Id));
            if(string.IsNullOrEmpty(activity.Provider))
                throw new ArgumentException("Provider must be provided for and activity", nameof(activity.Provider));

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
                uncommitedEvent.Actor = _streamId;

            _sink.Emit(uncommitedEvent);
            return Task.FromResult(true);
        }

        async Task<IReadOnlyCollection<CommitedActivityEvent>> SearchEventsAsync(string query, long checkpointToken, int maxCount)
        {
            var parsedQuery = _queryParser.Parse(query);
            var searchCriteria = parsedQuery.AsCriteria();

            var searchRequest = new SearchRequest
            {
                Count = maxCount,
                Criteria = searchCriteria.ToList(),
                OwnerId = _streamId
            };
            //todo: include checkpointToken and bucket vars into search request
            
            ISearchProvider searchProvider = _indexAccessor.NewIndexStore(new DefaultIndexCreationTask());
            var searchResults = searchProvider.Search(searchRequest);
            var checkpointTokens = new List<long>(searchResults.Count);

            foreach (dynamic item in searchResults)
            {
                checkpointTokens.Add(item.CheckpointToken);
            }

            return await _appendOnlyStore.ReadRecordsAsync(checkpointTokens);
        }
    }
}