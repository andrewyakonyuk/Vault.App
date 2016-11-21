using NEventStore;
using NEventStore.Dispatcher;

namespace Vault.Activity.Host
{
    public class EmptyDispatchCommits : IDispatchCommits
    {
        public void Dispatch(ICommit commit)
        {
        }

        public void Dispose()
        {
        }
    }
}