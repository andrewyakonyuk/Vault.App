using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Domain.Models.Activities;

namespace Vault.Shared.Connectors
{
    public class PullConnectionResult : IEnumerable<Activity>
    {
        readonly List<Activity> _activities;

        public readonly static PullConnectionResult Empty = new PullConnectionResult { IsCancellationRequested = true };

        public bool IsCancellationRequested { get; set; }

        public PullConnectionResult(IEnumerable<Activity> activities)
        {
            if (activities == null)
                throw new ArgumentNullException(nameof(activities));

            _activities = new List<Activity>(activities);
        }

        public PullConnectionResult()
        {
            _activities = new List<Activity>();
        }

        public IEnumerator<Activity> GetEnumerator()
        {
            return _activities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _activities.GetEnumerator();
        }
    }
}
