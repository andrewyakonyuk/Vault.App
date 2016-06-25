﻿using System;

namespace Vault.Shared.TransientFaultHandling
{
    // <summary>
    /// A retry strategy with a specified number of retry attempts and an incremental time interval between retries.
    /// </summary>
    public class Incremental : RetryStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> class.
        /// </summary>
        public Incremental() : this(RetryStrategy.DefaultClientRetryCount, RetryStrategy.DefaultRetryInterval, RetryStrategy.DefaultRetryIncrement)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> class with the specified retry settings.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <param name="initialInterval">The initial interval that will apply for the first retry.</param>
        /// <param name="increment">The incremental time value that will be used to calculate the progressive delay between retries.</param>
        public Incremental(int retryCount, TimeSpan initialInterval, TimeSpan increment) : this(null, retryCount, initialInterval, increment)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> class with the specified name and retry settings.
        /// </summary>
        /// <param name="name">The retry strategy name.</param>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <param name="initialInterval">The initial interval that will apply for the first retry.</param>
        /// <param name="increment">The incremental time value that will be used to calculate the progressive delay between retries.</param>
        public Incremental(string name, int retryCount, TimeSpan initialInterval, TimeSpan increment) : this(name, retryCount, initialInterval, increment, RetryStrategy.DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> class with the specified number of retry attempts, time interval, retry strategy, and fast start option.
        /// </summary>
        /// <param name="name">The retry strategy name.</param>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <param name="initialInterval">The initial interval that will apply for the first retry.</param>
        /// <param name="increment">The incremental time value that will be used to calculate the progressive delay between retries.</param>
        /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
        public Incremental(string name, int retryCount, TimeSpan initialInterval, TimeSpan increment, bool firstFastRetry) : base(name, firstFastRetry)
        {
            Guard.ArgumentNotNegativeValue(retryCount, "retryCount");
            Guard.ArgumentNotNegativeValue(initialInterval.Ticks, "initialInterval");
            Guard.ArgumentNotNegativeValue(increment.Ticks, "increment");
            this.retryCount = retryCount;
            this.initialInterval = initialInterval;
            this.increment = increment;
        }

        /// <summary>
        /// Returns the corresponding ShouldRetry delegate.
        /// </summary>
        /// <returns>The ShouldRetry delegate.</returns>
        public override ShouldRetry GetShouldRetry()
        {
            return delegate (int currentRetryCount, Exception lastException, out TimeSpan retryInterval)
            {
                if (currentRetryCount < this.retryCount)
                {
                    retryInterval = TimeSpan.FromMilliseconds(this.initialInterval.TotalMilliseconds + this.increment.TotalMilliseconds * (double)currentRetryCount);
                    return true;
                }
                retryInterval = TimeSpan.Zero;
                return false;
            };
        }

        private readonly TimeSpan increment;

        private readonly TimeSpan initialInterval;

        private readonly int retryCount;
    }
}