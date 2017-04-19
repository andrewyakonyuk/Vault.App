using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vault.Shared;

namespace Vault.Activity.Utility
{
    public class ScheduledTimer : IDisposable
    {
        private DateTime _next = DateTime.MaxValue;
        private DateTime _last = DateTime.MinValue;
        private readonly Timer _timer;
        private readonly ILogger _logger;
        private readonly IClock _clock;
        private readonly Func<Task<DateTime?>> _timerCallback;
        private readonly TimeSpan _minimumInterval;
        private bool _isRunning = false;
        private bool _shouldRunAgainImmediately = false;
        private object _lock = new object();

        public ScheduledTimer(
            Func<Task<DateTime?>> timerCallback,
            ILoggerFactory logger,
            IClock clock,
            TimeSpan? dueTime = null, 
            TimeSpan? minimumIntervalTime = null)
        {
            _logger = logger.CreateLogger<ScheduledTimer>();
            _clock = clock;
            _timerCallback = timerCallback ?? throw new ArgumentNullException(nameof(timerCallback));
            _minimumInterval = minimumIntervalTime ?? TimeSpan.Zero;

            int dueTimeMs = dueTime.HasValue ? (int)dueTime.Value.TotalMilliseconds : Timeout.Infinite;
            _timer = new Timer(s => RunCallbackAsync().GetAwaiter().GetResult(), null, dueTimeMs, Timeout.Infinite);
        }

        public void ScheduleNext(DateTime? utcDate = null)
        {
            var utcNow = _clock.UtcNow;
            if (!utcDate.HasValue || utcDate.Value < utcNow)
                utcDate = utcNow;

            _logger.LogInformation($"ScheduleNext called: value={utcDate.Value:O}");
            if (utcDate == DateTime.MaxValue)
            {
                _logger.LogInformation("Ignoring MaxValue");
                return;
            }

            lock (_lock)
            {
                // already have an earlier scheduled time
                if (_next > utcNow && utcDate > _next)
                {
                    _logger.LogInformation($"Ignoring because already scheduled for earlier time {utcDate.Value.Ticks} {_next.Ticks}");
                    return;
                }

                // ignore duplicate times
                if (_next == utcDate)
                {
                    _logger.LogInformation("Ignoring because already scheduled for same time");
                    return;
                }

                int delay = Math.Max((int)Math.Ceiling(utcDate.Value.Subtract(utcNow).TotalMilliseconds), 0);
                _next = utcDate.Value;
                if (_last == DateTime.MinValue)
                    _last = _next;

                _logger.LogInformation($"Scheduling next: delay={delay}");
                _timer.Change(delay, Timeout.Infinite);
            }
        }

        public void ScheduleNext(TimeSpan next)
        {
            int delay = Math.Max((int)Math.Ceiling(next.TotalMilliseconds), 0);
            var utcNow = _clock.UtcNow;
            _next =  utcNow.AddMilliseconds(delay);
            if (_last == DateTime.MinValue)
                _last = _next;

            _logger.LogInformation($"Scheduling next: delay={delay}");
            _timer.Change(delay, Timeout.Infinite);
        }

        private async Task RunCallbackAsync()
        {
            if (_isRunning)
            {
                _logger.LogInformation("Exiting run callback because its already running, will run again immediately.");
                _shouldRunAgainImmediately = true;
                return;
            }

            _logger.LogInformation("Starting RunCallbackAsync");
            lock (_lock)
            {
                if (_isRunning)
                {
                    _logger.LogInformation("Exiting run callback because its already running, will run again immediately.");
                    _shouldRunAgainImmediately = true;
                    return;
                }

                _last = _clock.UtcNow;
            }

            try
            {
                _isRunning = true;
                DateTime? next = null;

                try
                {
                    next = await _timerCallback();
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(0), ex, $"Error running scheduled timer callback: {ex.Message}");
                    _shouldRunAgainImmediately = true;
                }

                if (_minimumInterval > TimeSpan.Zero)
                {
                    _logger.LogInformation("Sleeping for minimum interval: {0}", _minimumInterval);
                    await _clock.SleepAsync((int)_minimumInterval.TotalMilliseconds, default(CancellationToken));
                    _logger.LogInformation("Finished sleeping");
                }

                var nextRun = _clock.UtcNow.AddMilliseconds(10);
                if (_shouldRunAgainImmediately || next.HasValue && next.Value <= nextRun)
                    ScheduleNext(nextRun);
                else if (next.HasValue)
                    ScheduleNext(next.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(0), ex, $"Error running schedule next callback: {ex.Message}");
            }
            finally
            {
                _isRunning = false;
                _shouldRunAgainImmediately = false;
            }

            _logger.LogInformation("Finished RunCallbackAsync");
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
