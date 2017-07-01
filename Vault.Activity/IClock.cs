using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vault.Activity
{
    public interface IClock
    {
        DateTimeOffset OffsetUtcNow { get; }
        DateTime UtcNow { get; }
        Task SleepAsync(int milliseconds, CancellationToken token);
    }

    public class DefaultClock : IClock
    {
        private static IClock _instance = new DefaultClock();
        public static IClock Instance
        {
            get => _instance ?? new DefaultClock();
            set => _instance = value;
        }

        public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;

        public DateTime UtcNow => DateTime.UtcNow;

        public Task SleepAsync(int milliseconds, CancellationToken token) => Task.Delay(milliseconds, token);
    }
}