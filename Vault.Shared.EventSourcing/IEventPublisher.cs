using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vault.Shared.EventSourcing
{
    public interface IEventPublisher
    {
        Task PublishAsync(IEnumerable<IEvent> events);
    }
}