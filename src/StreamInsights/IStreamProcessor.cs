using System.Threading;
using System.Threading.Tasks;
using StreamInsights.Abstractions;

namespace StreamInsights
{
    public interface IStreamProcessor
    {
        Task Process(CommitedActivity activity, NextStreamProcessor next, CancellationToken token);
    }

    public delegate Task NextStreamProcessor(CommitedActivity activity, CancellationToken token);
}
