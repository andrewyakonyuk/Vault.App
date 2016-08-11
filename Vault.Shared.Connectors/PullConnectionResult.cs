using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Activity.Commands;

namespace Vault.Shared.Connectors
{
    public class PullConnectionResult : IEnumerable<ActivityCommandBase>
    {
        readonly List<ActivityCommandBase> _activities;

        public readonly static PullConnectionResult Empty = new PullConnectionResult { IsCancellationRequested = true };

        public bool IsCancellationRequested { get; set; }

        public PullConnectionResult(IEnumerable<ActivityCommandBase> activities)
        {
            if (activities == null)
                throw new ArgumentNullException(nameof(activities));

            _activities = new List<ActivityCommandBase>(activities);
        }

        public PullConnectionResult()
        {
            _activities = new List<ActivityCommandBase>();
        }

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