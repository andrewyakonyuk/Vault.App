using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Activity.Persistence;
using Vault.Activity.Sinks;
using Vault.Activity.Utility;
using Vault.Shared.Search;
using Vault.Shared.Search.Parsing;

namespace Vault.Activity.Client
{
    public class DefaultActivityClient : IActivityClient
    {
        readonly ISink<UncommitedActivityEvent> _sink;
        readonly IClock _clock;
        readonly IAppendOnlyStore _appendOnlyStore;
        readonly ISearchProvider _searchProvider;
        readonly ISearchQueryParser _queryParser;

        public DefaultActivityClient(
            IEnumerable<ISink<UncommitedActivityEvent>> sinks,
            IAppendOnlyStore appendOnlyStore,
             ISearchProvider searchProvider,
            ISearchQueryParser queryParser,
            IClock clock)
        {
            _sink = new AggregateSink<UncommitedActivityEvent>(sinks);
            _clock = clock;
            _appendOnlyStore = appendOnlyStore;
            _searchProvider = searchProvider;
            _queryParser = queryParser;
        }

        public Task<IActivityFeed> GetFeedAsync(string bucket, Guid streamId)
        {
            if (string.IsNullOrEmpty(bucket))
                throw new ArgumentNullException(nameof(bucket));
            if (streamId == Guid.Empty)
                throw new ArgumentException("Stream id is not set", nameof(streamId));

            var activityFeed = new ActivityFeed(bucket, streamId, _sink, _appendOnlyStore, _searchProvider, _queryParser, _clock);

            return Task.FromResult<IActivityFeed>(activityFeed);
        }
    }
}
