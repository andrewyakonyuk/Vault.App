using Orleans.Providers.Streams.Common;

namespace Vault.Shared.EventStreams
{
    public class EventStoreStreamProvider : PersistentStreamProvider<EventStoreAdapterFactory>
    {
    }
}