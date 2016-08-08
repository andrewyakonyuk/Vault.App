using System;
using System.Collections.ObjectModel;

namespace Vault.Shared.EventSourcing
{
    public class ChildEntitiesCollection<TEntity> : Collection<TEntity>
        where TEntity : IEventProvider
    {
        readonly IRegisterChildEntities _aggregateRoot;

        public ChildEntitiesCollection(IRegisterChildEntities aggregateRoot)
        {
            if (aggregateRoot == null)
                throw new ArgumentNullException(nameof(aggregateRoot));

            _aggregateRoot = aggregateRoot;
        }

        protected override void InsertItem(int index, TEntity item)
        {
            base.InsertItem(index, item);
            _aggregateRoot.RegisterChildEventProvider(item);
        }
    }
}