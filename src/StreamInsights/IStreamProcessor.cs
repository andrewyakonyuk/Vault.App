using System.Threading;
using System.Threading.Tasks;
using StreamInsights.Abstractions;

namespace StreamInsights
{
    public interface IStreamProcessor
    {
        Task Process(CommitedActivity activity, StreamPipeline next, CancellationToken token);
    }

    public delegate Task StreamPipeline(CommitedActivity activity, CancellationToken token);
}
