using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Activity.Commands;

namespace Vault.Activity.Services.Connectors
{
    public class PullResult : IEnumerable<ActivityCommandBase>
    {
        readonly List<ActivityCommandBase> _activities;

        public readonly static PullResult Empty = new PullResult { IsCancellationRequested = true };

        public bool IsCancellationRequested { get; set; }

        public int Iteration { get; }

        public PullResult(IEnumerable<ActivityCommandBase> activities, int iteration)
        {
            if (activities == null)
                throw new ArgumentNullException(nameof(activities));

            _activities = new List<ActivityCommandBase>(activities);
            Iteration = iteration;
        }

        public PullResult()
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