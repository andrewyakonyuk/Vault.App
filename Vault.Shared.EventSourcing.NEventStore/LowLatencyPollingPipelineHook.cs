using System;
using NEventStore;
using NEventStore.Client;

namespace Vault.Shared.EventSourcing.NEventStore
{
    public class LowLatencyPollingPipelineHook : PipelineHookBase
    {
        private readonly Lazy<IObserveCommits> _commitsObserver;

        public LowLatencyPollingPipelineHook(Lazy<IObserveCommits> commitsObserver)
        {
            if (commitsObserver == null)
                throw new ArgumentNullException(nameof(commitsObserver));

            _commitsObserver = commitsObserver;
        }

        public override void PostCommit(ICommit committed)
        {
            base.PostCommit(committed);
            _commitsObserver.Value.PollNow();
        }
    }
}