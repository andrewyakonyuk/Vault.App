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
using Vault.Activity.Sinks;
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
using Vault.Activity.Client;
using Vault.Activity.Persistence;
using Vault.Activity.Utility;
using Vault.Activity.Indexes;

namespace Vault.Activity.Host
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
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
            
            services.AddSingleton<ISearchResultTransformer, DefaultSearchResultTransformer>();
            services.AddTransient<IIndexDocumentTransformer, DefaultIndexDocumentTransformer>();
            services.AddTransient<IIndexWriterInitializer, IndexWriterInitializer>();
            services.AddSingleton<IIndexWriterAccessor, DefaultIndexWriterAccessor>();
            services.AddTransient<ISearchQueryParser, DefaultSearchQueryParser>();
            services.AddSingleton<IIndexStoreAccessor, DefaultIndexStoreAccessor>();
            services.AddQueries();

            services.AddSingleton<IAppendOnlyStore, SqlAppendOnlyStore>();
            services.AddSingleton<ISqlConnectionFactory, PostgreSqlConnectionFactory>(_ => new PostgreSqlConnectionFactory(configuration["connectionStrings:db"]));

            services.AddSingleton<ISink<UncommitedActivityEvent>, PluggableBatchingSink<UncommitedActivityEvent>>(s =>
            {
                var store = s.GetRequiredService<IAppendOnlyStore>();
                var adapter = new AppendOnlyStoreBatchingAdapter(store);

                var clock = s.GetRequiredService<IClock>();
                var logger = s.GetRequiredService<ILogger>();
                return new PluggableBatchingSink<UncommitedActivityEvent>(adapter, logger, clock);
            });
            services.AddSingleton<ISink<CommitedActivityEvent>, PluggableBatchingSink<CommitedActivityEvent>>(s =>
            {
                var indexAccessor = s.GetRequiredService<IIndexStoreAccessor>();
                var indexTasks = s.GetServices<AbstractIndexCreationTask<CommitedActivityEvent>>();

                var adapter = new IndexBatchingAdapter<CommitedActivityEvent>(indexAccessor, indexTasks);
                var clock = s.GetRequiredService<IClock>();
                var logger = s.GetRequiredService<ILogger>();
                return new PluggableBatchingSink<CommitedActivityEvent>(adapter, logger, clock);
            });
            services.AddSingleton<IActivityClient, DefaultActivityClient>();
            services.AddSingleton<JsonSerializer>(_ => new JsonSerializer());
            services.AddSingleton<AbstractIndexCreationTask<CommitedActivityEvent>, DefaultIndexCreationTask>();

            return services.BuildServiceProvider();
        }
    }
}