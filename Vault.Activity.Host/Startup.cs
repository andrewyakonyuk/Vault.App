using System;
using System.ComponentModel;
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
using Vault.Activity.Services;
using Vault.Activity.Services.Connectors;
using Vault.Shared;
using Vault.Shared.Connectors.Pocket;
using Vault.Shared.EventSourcing;
using Vault.Shared.EventSourcing.NEventStore;
using Vault.Shared.Search;
using Vault.Shared.Search.Lucene;
using Vault.Shared.Search.Lucene.Converters;
using Vault.Shared.Search.Parsing;

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

            services.AddTransient<ISearchProvider, LuceneSearchProvider>();
            services.AddSingleton<ISearchResultTransformer, DefaultSearchResultTransformer>();
            services.AddSingleton<IReportUnitOfWorkFactory>(s =>
                new DefaultReportUnitOfWorkFactory(
                    new LuceneUnitOfWorkFactory(
                        s.GetRequiredService<IIndexWriterAccessor>(),
                        s.GetRequiredService<IIndexDocumentTransformer>(),
                        s.GetRequiredService<IIndexDocumentMetadataProvider>())));
            services.AddTransient<IIndexDocumentTransformer, DefaultIndexDocumentTransformer>();
            services.AddTransient<IIndexWriterInitializer, IndexWriterInitializer>();
            services.AddSingleton<IIndexWriterAccessor, DefaultIndexWriterAccessor>();
            services.AddTransient<ISearchQueryParser, DefaultSearchQueryParser>();

            var builder = new FluentDescriptorProviderBuilder()
                .Field("Id", "_id", converter: new Int32Converter())
                .Field("OwnerId", "_ownerId", isKey: true, converter: new Int32Converter())
                .Field("ResourceId", isKey: true)
                .Field("ServiceName", isKey: true)
                .Field("DocumentType", "_documentType", isKey: true, isAnalysed: true)
                .Field("Published", "_published", converter: new LuceneDateTimeConverter())
                .Field("StartDate", converter: new LuceneDateTimeConverter())
                .Field("EndDate", converter: new LuceneDateTimeConverter())
                .Field("Duration", converter: new TimeSpanConverter())
                .Field("Name", isKeyword: true, isAnalysed: true)
                .Field("Description", isKeyword: true, isAnalysed: true)
                .Field("Elevation", converter: new DoubleConverter())
                .Field("Latitude", converter: new DoubleConverter())
                .Field("Longitude", converter: new DoubleConverter())
                .Field("ByArtist", isKeyword: true, isAnalysed: true)
                .Field("InAlbum", isKeyword: true, isAnalysed: true)
                .Field("Body", isAnalysed: true)
                .Field("Summary", isAnalysed: true)
                .Field("Thumbnail")
                .Field("Url", isAnalysed: true, isKeyword: true);
            services.AddSingleton<IIndexDocumentMetadataProvider>(s => builder.Build());

            return services.BuildServiceProvider();
        }
    }
}