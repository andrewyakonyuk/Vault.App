using System;
using System.Collections;
using System.Collections.Generic;

namespace Vault.Activity.Services.Connectors
{
    public class CatchResult : IEnumerable<ActivityEvent>
    {
        readonly List<ActivityEvent> _activities;

        public readonly static CatchResult Empty = new CatchResult { };

        public CatchResult(IEnumerable<ActivityEvent> activities)
        {
            if (activities == null)
                throw new ArgumentNullException(nameof(activities));

            _activities = new List<ActivityEvent>(activities);
        }

        CatchResult()
        {
            _activities = new List<ActivityEvent>();
        }

        public int Count { get { return _activities.Count; } }

        public IEnumerator<ActivityEvent> GetEnumerator()
        {
            return _activities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _activities.GetEnumerator();
        }
    }
}