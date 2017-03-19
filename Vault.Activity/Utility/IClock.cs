using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vault.Activity.Utility
{
    public interface IClock
    {
        DateTimeOffset OffsetUtcNow { get; }
        DateTime UtcNow { get; }
        Task SleepAsync(int milliseconds, CancellationToken token);
    }

    public class DefaultClock : IClock
    {
        public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;

        public DateTime UtcNow => DateTime.UtcNow;

        public Task SleepAsync(int milliseconds, CancellationToken token) => Task.Delay(milliseconds, token);
    }
}