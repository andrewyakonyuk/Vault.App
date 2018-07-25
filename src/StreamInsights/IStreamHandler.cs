using StreamInsights.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace StreamInsights
{
    public interface IStreamHandler
    {
        Task Handle(CommitedActivity activity, CancellationToken token, NextStreamHandler next);
    }

    public delegate Task NextStreamHandler(CommitedActivity activity, CancellationToken token);
}
