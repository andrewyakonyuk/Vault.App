using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vault.Shared.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync(IEnumerable<IEvent> events);
    }
}