using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Activity.Services.Connectors
{
    public class PullResult : IEnumerable<ActivityEvent>
    {
        readonly List<ActivityEvent> _activities;

        public readonly static PullResult Empty = new PullResult { IsCancellationRequested = true };

        public bool IsCancellationRequested { get; set; }

        public int Iteration { get; }

        public PullResult(IEnumerable<ActivityEvent> activities, int iteration)
        {
            if (activities == null)
                throw new ArgumentNullException(nameof(activities));

            _activities = new List<ActivityEvent>(activities);
            Iteration = iteration;
        }

        public PullResult()
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