using NEventStore;
using NEventStore.Dispatcher;
using System;

namespace Vault.Shared.NEventStore
{
    public class ReadModelCommitObserver : IObserver<ICommit>
    {
        private readonly ICheckpointRepository _checkpointRepo;
        private readonly IDispatchCommits _dispatcher;
        private readonly ILogger _logger;

        public ReadModelCommitObserver(ICheckpointRepository checkpointRepo,
            IDispatchCommits dispatcher,
            ILogger logger)
        {
            if (checkpointRepo == null)
                throw new ArgumentNullException(nameof(checkpointRepo));
            if (dispatcher == null)
                throw new ArgumentNullException(nameof(dispatcher));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _checkpointRepo = checkpointRepo;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        public void OnCompleted()
        {
            _logger.WriteInfo("commit observation completed.");
        }

        public void OnError(Exception error)
        {
            _logger.WriteInfo("Exception from ReadModelCommitObserver: {0}", error.Message);
            throw error;
        }

        public void OnNext(ICommit commit)
        {
            if (commit == null)
                throw new ArgumentNullException("commit");

            _dispatcher.Dispatch(commit);
            _checkpointRepo.SaveCheckpoint(commit.CheckpointToken);
        }
    }
}