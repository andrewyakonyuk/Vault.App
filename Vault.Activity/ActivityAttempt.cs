using System;
using Orleans.Concurrency;
using Vault.Shared;

namespace Vault.Activity
{
    [Serializable]
    public class ActivityAttempt : EventAttempt<ActivityEvent>
    {
        public ActivityAttempt()
        {
        }

        public ActivityAttempt(ActivityEvent body)
            : base(body)
        {
            Body = body;
        }

        public static ActivityAttempt Create(ActivityEvent body)
        {
            return new ActivityAttempt(body);
        }
    }
}