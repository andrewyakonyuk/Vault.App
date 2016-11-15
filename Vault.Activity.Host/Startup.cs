using System;
using System.Collections.Generic;
using System.Data;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NEventStore;
using NEventStore.Client;
using NEventStore.Dispatcher;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using Npgsql;
using Vault.Activity.Services;
using Vault.Activity.Services.Connectors;
using Vault.Shared;
using Vault.Shared.Connectors.Pocket;
using Vault.Shared.EventSourcing;
using Vault.Shared.EventSourcing.NEventStore;

namespace Vault.Activity.Host
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddTransient<IRepository, EventStoreRepository>();
            services.AddTransient<IDetectConflicts, ConflictDetector>();
            services.AddTransient<IConstructAggregates, AggregateFactory>();
            services.AddSingleton<IPipelineHook, LowLatencyPollingPipelineHook>();
            services.AddSingleton<IStoreEvents>(s => s.GetRequiredService<IEventStoreInitializer>().Create());
            services.AddSingleton<ICheckpointRepository, InMemoryCheckpointRepository>();
            services.AddTransient<IObserver<ICommit>, ReadModelCommitObserver>();
            services.AddTransient<EventObserverSubscriptionFactory, EventObserverSubscriptionFactory>();
            services.AddSingleton<IObserveCommits>(s => s.GetRequiredService<EventObserverSubscriptionFactory>().Construct());
            services.AddTransient<IDispatchCommits, DistributedDispatchCommits>();
            services.AddTransient<Lazy<IObserveCommits>>(s => new Lazy<IObserveCommits>(() => s.GetRequiredService<IObserveCommits>()));
            services.AddTransient<IEventedUnitOfWorkFactory, EventedUnitOfWorkFactory>();
            services.AddTransient<IEventsRebuilder, EventsRebuilder>();
            services.AddSingleton<IEventStoreInitializer, NEventStoreWithCustomPipelineFactory>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                 .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                 .AddEnvironmentVariables()
                 .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddSingleton<IOptions<PocketConnectionOptions>>(_ =>
            {
                var options = new PocketConnectionOptions
                {
                    ConsumerKey = configuration["authentication:pocket:consumerKey"]
                };

                return Options.Create<PocketConnectionOptions>(options);
            });

            services.Configure<PocketConnectionOptions>(options => options.ConsumerKey = configuration["authentication:pocket:consumerKey"]);
            services.AddTransient<IPullConnectionProvider, PocketConnectionProvider>();

            services.AddSingleton<IConnectionPool<IPullConnectionProvider>, DefaultConnectionPool<IPullConnectionProvider>>();
            services.AddSingleton<IConnectionPool<ICatchConnectionProvider>, DefaultConnectionPool<ICatchConnectionProvider>>();

            services.AddSingleton<IClock, DefaultClock>();

            return services.BuildServiceProvider();
        }
    }
}