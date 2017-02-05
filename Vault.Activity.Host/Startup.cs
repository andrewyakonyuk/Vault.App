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
using Vault.Shared.Search;
using Vault.Shared.Search.Lucene;
using Vault.Shared.Search.Lucene.Converters;
using Vault.Shared.Search.Parsing;
using Orleans.Serialization;
using System.Globalization;
using System.Collections.Generic;
using MultilineStringConverter = Vault.Shared.Search.Lucene.Converters.MultilineStringConverter;

namespace Vault.Activity.Host
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            SerializationManager.Register(typeof(ActivityEvent), typeof(ActivityEventSerializer));

            services.AddSingleton<ILogger, ConsoleLogger>();
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
            services.AddSingleton<IIndexUnitOfWorkFactory, LuceneUnitOfWorkFactory>();
            services.AddTransient<IIndexDocumentTransformer, DefaultIndexDocumentTransformer>();
            services.AddTransient<IIndexWriterInitializer, IndexWriterInitializer>();
            services.AddSingleton<IIndexWriterAccessor, DefaultIndexWriterAccessor>();
            services.AddTransient<ISearchQueryParser, DefaultSearchQueryParser>();
            services.AddQueries();

            var builder = new FluentDescriptorProviderBuilder()
                .Index(IndexNames.Default)
                    .Field(nameof(ActivityEvent.Id), "_id", isKey: true)
                    .Field(nameof(ActivityEvent.Actor), "_ownerId", isKey: true, converter: new StringConverter())
                    .Field(nameof(ActivityEvent.Provider), isKey: true)
                    .Field(nameof(ActivityEvent.Verb), "_verb", isKey: true, isAnalysed: true)
                    .Field(nameof(ActivityEvent.Published), "_published", converter: new LuceneDateTimeConverter())
                    .Field(nameof(ActivityEvent.Title), isKeyword: true, isAnalysed: true)
                    .Field(nameof(ActivityEvent.Content), isKeyword: true, isAnalysed: true)
                    .Field(nameof(ActivityEvent.Target))
                    .Field(nameof(ActivityEvent.Uri), isAnalysed: true, isKeyword: true)
                    .Field("Tags", "_tags", converter: new MultilineStringConverter(), isKeyword: true, isAnalysed: true)
                    .BuildIndex();
            services.AddSingleton<IIndexDocumentMetadataProvider>(s => builder.BuildProvider());

            return services.BuildServiceProvider();
        }
    }
}