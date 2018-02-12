using MediatR;

namespace Vault.WebApp.Infrastructure.MediatR
{
    public class Notification<T> : INotification
    {
        public Notification(T payload)
        {
            Payload = payload;
        }

        public T Payload { get; }
    }

    public class Notification
    {
        public static Notification<T> Create<T>(T payload)
        {
            return new Notification<T>(payload);
        }
    }
}
