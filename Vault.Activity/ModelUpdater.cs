using System;
using System.Threading;
using CommonDomain.Core;
using CommonDomain.Persistence;
using Vault.Shared.Domain;
using Vault.Shared.TransientFaultHandling;

namespace Vault.Activity
{
    public class ModelUpdater<T>
        where T : AggregateBase
    {
        readonly Func<ResourceKey, T> _modelFactory;
        readonly IResourceKeyMapper _keyMapper;
        readonly IRepository _repository;
        static readonly object updaterLock = new object();

        public ModelUpdater(
            Func<ResourceKey, T> modelFactory,
            IRepository repository,
            IResourceKeyMapper keyMapper)
        {
            if (modelFactory == null)
                throw new ArgumentNullException(nameof(modelFactory));

            _modelFactory = modelFactory;
            _repository = repository;
            _keyMapper = keyMapper;
        }

        public void Update(ResourceKey key, Action<T> modify)
        {
            var retryStrategy = RetryStrategy.DefaultFixed;
            var retryPolicy = new RetryPolicy<ModelUpdaterStrategy>(retryStrategy);

            retryPolicy.ExecuteAction(() => UpdateCore(key, modify));
        }

        protected virtual void UpdateCore(ResourceKey key, Action<T> modify)
        {
            Guid modelId;
            T model;

            if (_keyMapper.TryResolveId(key, out modelId))
                model = _repository.GetById<T>("activity", modelId);
            else
            {
                lock (updaterLock)
                {
                    if (_keyMapper.TryResolveId(key, out modelId))
                        model = _repository.GetById<T>("activity", modelId);
                    else
                    {
                        model = _modelFactory(key);
                        _keyMapper.Link(key, model.Id);
                    }
                }
            }

            if (model == null)
                throw new EntityNotFoundException($"Entity with key '{key}' could not be found");

            modify(model);

            _repository.Save("activity", model, Guid.NewGuid());
        }

        private class ModelUpdaterStrategy : ITransientErrorDetectionStrategy
        {
            public bool IsTransient(Exception ex)
            {
                return ex is NEventStore.ConcurrencyException;
            }
        }
    }
}