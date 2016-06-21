using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using Microsoft.Extensions.DependencyInjection;
using NEventStore;
using NEventStore.Client;
using NEventStore.Dispatcher;
using System;

namespace Vault.Shared.NEventStore
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEventStore(this IServiceCollection services)
        {
            services.AddTransient<IRepository, EventStoreRepository>();
            services.AddTransient<IDetectConflicts, ConflictDetector>();
            services.AddTransient<IConstructAggregates, AggregateFactory>();
            services.AddSingleton<IPipelineHook, LowLatencyPollingPipelineHook>();
            services.AddSingleton<IStoreEvents>(s => s.GetRequiredService<IEventStoreInitializer>().Create());
            services.AddSingleton<ICheckpointRepository, InMemoryCheckpointRepository>();
            services.AddTransient<IObserver<ICommit>, ReadModelCommitObserver>();
            services.AddTransient<EventObserverSubscriptionFactory, EventObserverSubscriptionFactory>();
            services.AddSingleton<IObserveCommits>(s => s.GetRequiredService<EventObserverSubscriptionFactory>().Construct());
            services.AddTransient<IDispatchCommits, InMemoryDispatcher>();
            services.AddTransient<Lazy<IObserveCommits>>(s => new Lazy<IObserveCommits>(() => s.GetRequiredService<IObserveCommits>()));
            services.AddTransient<IEventedUnitOfWorkFactory, EventedUnitOfWorkFactory>();
            services.AddTransient<IEventsRebuilder, EventsRebuilder>();
        }
    }
}