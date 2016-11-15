using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;
using Vault.Activity.Events;

namespace Vault.Activity.Services.ActivityLog
{
    public interface IPushActivityNotifier<TActivity> : IGrainWithGuidKey
    {
        Task NewActivityAsync(Immutable<TActivity> activity);

        Task CompleteAsync();
    }

    public class PushActivityNotifier
    {
        public const string CommandStreamNamespace = "command-activity-log";
        public const string EventStreamNamespace = "event-activity-log";
    }

    public class PushActivityNotifier<TActivity> : Grain, IPushActivityNotifier<TActivity>
    {
        public static string StreamNamespace
        {
            get
            {
                return typeof(ActivityEventBase).IsAssignableFrom(typeof(TActivity))
                    ? PushActivityNotifier.EventStreamNamespace
                    : PushActivityNotifier.CommandStreamNamespace;
            }
        }

        IAsyncStream<TActivity> _stream;

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            var streamProvider = base.GetStreamProvider("ActivityProvider");
            _stream = streamProvider.GetStream<TActivity>(this.GetPrimaryKey(), StreamNamespace);
        }

        public Task CompleteAsync()
        {
            return _stream.OnCompletedAsync();
        }

        public Task NewActivityAsync(Immutable<TActivity> activity)
        {
            return _stream.OnNextAsync(activity.Value);
        }
    }
}