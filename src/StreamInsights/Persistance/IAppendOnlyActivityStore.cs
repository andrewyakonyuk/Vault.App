using StreamInsights.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamInsights.Persistance
{
    public interface IAppendOnlyActivityStore
    {
        Task AppendAsync(IReadOnlyCollection<UncommitedActivity> events, CancellationToken cancellationToken = default(CancellationToken));

        Task<IReadOnlyCollection<CommitedActivity>> ReadAsync(string streamId, string bucket, long checkpointToken, int maxCount, CancellationToken cancellationToken = default(CancellationToken));

        Task<IReadOnlyCollection<CommitedActivity>> ReadAsync(long checkpointToken, int maxCount, CancellationToken cancellationToken = default(CancellationToken));
    }
}
