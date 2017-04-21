using System;
using System.Collections;
using System.Collections.Generic;
using Vault.Shared.Activity;

namespace Vault.Activity.Services.Connectors
{
    public class CatchResult : IEnumerable<ActivityEventAttempt>
    {
        readonly List<ActivityEventAttempt> _activities;

        public readonly static CatchResult Empty = new CatchResult { };

        public CatchResult(IEnumerable<ActivityEventAttempt> activities)
        {
            if (activities == null)
                throw new ArgumentNullException(nameof(activities));

            _activities = new List<ActivityEventAttempt>(activities);
        }

        CatchResult()
        {
            _activities = new List<ActivityEventAttempt>();
        }

        public int Count { get { return _activities.Count; } }

        public IEnumerator<ActivityEventAttempt> GetEnumerator()
        {
            return _activities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _activities.GetEnumerator();
        }
    }
}