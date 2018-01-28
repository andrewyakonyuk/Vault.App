using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamInsights.Abstractions
{
    public interface IActivityStream
    {
        Task PushActivityAsync(Activity activity, CancellationToken token = default(CancellationToken));

        Task<IReadOnlyCollection<CommitedActivity>> ReadActivityAsync(long checkpointToken, int maxCount, CancellationToken token = default(CancellationToken));
    }
}
