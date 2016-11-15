using System;
using System.Collections;
using System.Collections.Generic;
using Vault.Activity.Commands;

namespace Vault.Activity.Services.Connectors
{
    public class CatchResult : IEnumerable<ActivityCommandBase>
    {
        readonly List<ActivityCommandBase> _activities;

        public readonly static CatchResult Empty = new CatchResult { };

        public CatchResult(IEnumerable<ActivityCommandBase> activities)
        {
            if (activities == null)
                throw new ArgumentNullException(nameof(activities));

            _activities = new List<ActivityCommandBase>(activities);
        }

        CatchResult()
        {
            _activities = new List<ActivityCommandBase>();
        }

        public int Count { get { return _activities.Count; } }

        public IEnumerator<ActivityCommandBase> GetEnumerator()
        {
            return _activities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _activities.GetEnumerator();
        }
    }
}