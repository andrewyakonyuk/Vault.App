namespace Vault.Shared.Events
{
    public interface IHandle<T> : IHandle
        where T : IEvent
    {
        void Handle(T @event);
    }

    public interface IHandle
    {
    }
}