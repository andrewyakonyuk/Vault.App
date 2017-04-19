﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Activity.Indexes;
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
        readonly IIndexStoreAccessor _indexAccessor;
        readonly ISearchQueryParser _queryParser;

        public DefaultActivityClient(
            IEnumerable<ISink<UncommitedActivityEvent>> sinks,
            IAppendOnlyStore appendOnlyStore,
            IIndexStoreAccessor indexAccessor,
            ISearchQueryParser queryParser,
            IClock clock)
        {
            _sink = new AggregateSink<UncommitedActivityEvent>(sinks);
            _clock = clock;
            _appendOnlyStore = appendOnlyStore;
            _indexAccessor = indexAccessor;
            _queryParser = queryParser;
        }

        public Task<IActivityFeed> GetFeedAsync(string bucket, Guid streamId)
        {
            if (string.IsNullOrEmpty(bucket))
                throw new ArgumentNullException(nameof(bucket));
            if (streamId == Guid.Empty)
                throw new ArgumentException("Stream id is not set", nameof(streamId));

            var activityFeed = new ActivityFeed(bucket, streamId, _sink, _appendOnlyStore, _indexAccessor, _queryParser, _clock);

            return Task.FromResult<IActivityFeed>(activityFeed);
        }
    }
}