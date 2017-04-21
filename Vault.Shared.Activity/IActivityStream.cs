﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Shared.Activity
{
    public interface IActivityStream
    {
        Task PushActivityAsync(ActivityEventAttempt activity);

        Task<IReadOnlyCollection<CommitedActivityEvent>> ReadEventsAsync(long checkpointToken, int maxCount);

        Task<IReadOnlyCollection<CommitedActivityEvent>> SearchEventsAsync(string query, long checkpointToken, int maxCount);
    }
}
