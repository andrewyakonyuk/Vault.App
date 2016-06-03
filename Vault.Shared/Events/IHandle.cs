namespace Vault.Shared.Events
{
    using System.Threading.Tasks;

    public interface IHandle<T> : IHandle
        where T : IEvent
    {
        Task HandleAsync(T @event);
    }

    public interface IHandle
    {
    }
}