using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared;

namespace Vault.Activity.Persistence
{
    public interface IAppendOnlyStore
    {
        Task AppendAsync(IEnumerable<UncommitedActivityEvent> events);

        Task<IReadOnlyCollection<CommitedActivityEvent>> ReadRecordsAsync(long checkpointToken, int maxCount);

        Task<IReadOnlyCollection<CommitedActivityEvent>> ReadRecordsAsync(Guid streamId, string bucket, long checkpointToken, int maxCount);

        Task<IReadOnlyCollection<CommitedActivityEvent>> ReadRecordsAsync(IList<long> checkpointTokens);
    }
}
