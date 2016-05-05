using System;
using System.Threading.Tasks;
using Vault.Shared.Domain;

namespace Vault.Shared.Events
{
    public static class EventPublisherExtensions
    {
        public static Task EntityCreated<T>(this IEventPublisher publisher, T entity)
            where T : IEntity
        {
            if (publisher == null)
                throw new ArgumentNullException(nameof(publisher));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return publisher.PublishAsync(new[] { new EntityCreated<T>(entity) });
        }

        public static Task EntityUpdated<T>(this IEventPublisher publisher, T entity)
          where T : IEntity
        {
            if (publisher == null)
                throw new ArgumentNullException(nameof(publisher));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return publisher.PublishAsync(new[] { new EntityUpdated<T>(entity) });
        }

        public static Task EntityDeleted<T>(this IEventPublisher publisher, T entity)
          where T : IEntity
        {
            if (publisher == null)
                throw new ArgumentNullException(nameof(publisher));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return publisher.PublishAsync(new[] { new EntityDeleted<T>(entity) });
        }
    }

    public class EntityCreated<T> : IEvent
        where T : IEntity
    {
        public EntityCreated(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; private set; }
    }

    public class EntityUpdated<T> : IEvent
      where T : IEntity
    {
        public EntityUpdated(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; private set; }
    }

    public class EntityDeleted<T> : IEvent
     where T : IEntity
    {
        public EntityDeleted(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; private set; }
    }
}