using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamInsights
{
    public interface IClock
    {
        DateTimeOffset UtcNowOffset { get; }
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

        public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;

        public DateTime UtcNow => DateTime.UtcNow;

        public Task SleepAsync(int milliseconds, CancellationToken token) => Task.Delay(milliseconds, token);
    }
}
