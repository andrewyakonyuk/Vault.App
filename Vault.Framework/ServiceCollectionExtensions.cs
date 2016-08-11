using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Vault.Framework.Api.Boards;
using Vault.Framework.Search.Parsing;
using Vault.Framework.Security;
using Vault.Shared.Commands;
using Vault.Shared.Connectors;
using Vault.Shared.EventSourcing;
using Vault.Shared.Identity;
using Vault.Shared.Queries;
using Vault.Shared.Search;
using Vault.Shared.Search.Lucene;
using Vault.Shared.Search.Lucene.Analyzers;
using Vault.Shared.Search.Lucene.Converters;

namespace Vault.Framework
{
    public static class ServiceCollectionExtensions
    {
        public static void AddVaultFramework(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationService, DefaultAuthorizationService>();
            services.AddScoped<IAuthorizer, DefaultAuthorizer>();
            services.AddScoped<WorkContext, WorkContext>();
            services.AddSingleton<IWorkContextAccessor, DefaultWorkContextAccessor>();
            services.AddSingleton<IEventPublisher, ImMemoryEventPublisher>();
            services.AddScoped<IBoardsApi, BoardsApi>();
            services.AddSearch();

            services.AddQueries();
            services.AddCommands();
            services.AddHandles();
            services.AddScheduler();
        }

        public static void AddQueries(this IServiceCollection services, Func<Assembly, bool> ignoredAssemblies = null)
        {
            services.AddSingleton<IQueryBuilder, QueryBuilder>();
            services.AddSingleton<IQueryFactory>(s => new DefaultQueryFactory(s));

            ignoredAssemblies = ignoredAssemblies ?? (_ => false);
            var query = typeof(IQuery<,>);

            foreach (var queryType in AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !ignoredAssemblies(assembly))
                .SelectMany(t => t.GetTypes())
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == query) && t.IsClass && !t.IsAbstract && !t.Name.Contains("CachedQuery"))
                .ToList())
            {
                foreach (var queryInterfaceType in queryType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == query))
                {
                    services.AddTransient(queryInterfaceType, queryType);
                }
            }
        }

        public static void AddCommands(this IServiceCollection services, Func<Assembly, bool> ignoredAssemblies = null)
        {
            services.AddSingleton<ICommandBuilder, CommandBuilder>();
            services.AddSingleton<ICommandFactory>(s => new DefaultCommandFactory(s));

            ignoredAssemblies = ignoredAssemblies ?? (_ => false);

            var command = typeof(ICommand<>);

            foreach (var commandImplType in AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !ignoredAssemblies(assembly))
                .SelectMany(t => t.GetTypes())
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == command) && t.IsClass)
                .ToList())
            {
                foreach (var commandInterfaceType in commandImplType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == command))
                {
                    services.AddTransient(commandInterfaceType, commandImplType);
                }
            }
        }

        public static void AddHandles(this IServiceCollection services, Func<Assembly, bool> ignoredAssemblies = null)
        {
            services.AddSingleton<IEventHandlerFactory, DefaultEventHandlerFactory>();

            ignoredAssemblies = ignoredAssemblies ?? (_ => false);

            var handler = typeof(IHandle<>);
            foreach (var group in AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !ignoredAssemblies(assembly))
                .SelectMany(t => t.GetTypes())
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handler) && t.IsClass)
                .ToList().GroupBy(implType => implType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handler), implType => implType))
            {
                foreach (var handlerInterfaceType in group.Key)
                {
                    foreach (var handlerImplType in group)
                    {
                        services.Add(new ServiceDescriptor(handlerInterfaceType, handlerImplType, ServiceLifetime.Transient));
                    }
                }
            }
        }

        public static void AddSearch(this IServiceCollection services)
        {
            services.AddTransient<ISearchProvider, LuceneSearchProvider>();
            services.AddSingleton<ISearchResultTransformer, DefaultSearchResultTransformer>();
            services.AddTransient<IReportUnitOfWorkFactory>(s =>
                new DefaultReportUnitOfWorkFactory(
                    new LuceneUnitOfWorkFactory(
                        s.GetRequiredService<IIndexWriterAccessor>(),
                        s.GetRequiredService<IIndexDocumentTransformer>(),
                        s.GetRequiredService<IIndexDocumentMetadataProvider>())));
            services.AddTransient<IIndexDocumentTransformer, DefaultIndexDocumentTransformer>();
            services.AddTransient<IIndexWriterInitializer, IndexWriterInitializer>();
            services.AddSingleton<IIndexWriterAccessor, DefaultIndexWriterAccessor>();

            var builder = new FluentDescriptorProviderBuilder()
                .Field("Id", "_id", isKey: true, converter: new Int32Converter())
                .Field("OwnerId", "_ownerId", isKey: true, converter: new Int32Converter())
                .Field("Published", "_published", converter: new LuceneDateTimeConverter())
                .Field("DocumentType", "_documentType", isKey: true, isAnalysed: true)
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
            services.AddTransient<ISearchQueryParser, DefaultSearchQueryParser>();
        }

        public static void AddScheduler(this IServiceCollection services)
        {
            services.AddSingleton<ServiceJobFactory>();
            services.AddSingleton(s =>
            {
                var schedulerFactory = new StdSchedulerFactory();
                IScheduler scheduler = schedulerFactory.GetScheduler();
                scheduler.JobFactory = s.GetRequiredService<ServiceJobFactory>();
                scheduler.Start();
                return scheduler;
            });

            services.AddTransient<PullConnectionJob>();
        }
    }

    internal class DefaultQueryFactory : IQueryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultQueryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IQuery<TCriterion, TResult> Create<TCriterion, TResult>()
            where TCriterion : ICriterion
        {
            var query = _serviceProvider.GetRequiredService<IQuery<TCriterion, TResult>>();
            var memoryCache = _serviceProvider.GetRequiredService<IMemoryCache>();
            return new CachedQuery<TCriterion, TResult>(query, memoryCache);
        }
    }

    internal class DefaultCommandFactory : ICommandFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultCommandFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICommand<TCommandContext> Create<TCommandContext>()
            where TCommandContext : ICommandContext
        {
            return _serviceProvider.GetRequiredService<ICommand<TCommandContext>>();
        }
    }

    internal class IndexWriterInitializer : IIndexWriterInitializer
    {
        public const string InMemory = ":memory:";

        readonly IConfiguration _configuration;
        readonly IHostingEnvironment _environment;

        public IndexWriterInitializer(
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public LuceneIndexWriter Create()
        {
            Lucene.Net.Store.Directory directory;
            var shouldCreate = true;

            if (_configuration["connectionStrings:index"] == InMemory)
            {
                directory = new RAMDirectory();
            }
            else
            {
                var pathToIndex = _configuration["connectionStrings:index"];
                directory = FSDirectory.Open(pathToIndex);
                shouldCreate = !((FSDirectory)directory).Directory.Exists || !directory.ListAll().Any();
            }

            var analyzer = new PerFieldAnalyzer(new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            analyzer.AddAnalyzer("keywords", new KeywordsAnalyzer(Lucene.Net.Util.Version.LUCENE_30, new string[0]));

            var writer = new LuceneIndexWriter(directory, analyzer, shouldCreate, IndexWriter.MaxFieldLength.UNLIMITED);
            writer.Commit();

            return writer;
        }
    }

    internal class ImMemoryEventPublisher : IEventPublisher
    {
        readonly IEventHandlerFactory _eventHandlerFactory;

        public ImMemoryEventPublisher(IEventHandlerFactory eventHandlerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory;
        }

        public async Task PublishAsync(IEnumerable<IEvent> events)
        {
            foreach (dynamic @event in events)
            {
                var handlers = _eventHandlerFactory.GetHandlers(@event);
                foreach (dynamic handler in handlers)
                {
                    await handler.HandleAsync(@event);
                }
            }
        }
    }

    public class NewUserLoginHandler : IHandle<EntityCreated<IdentityUserLogin>>
    {
        IScheduler _scheduler;

        public NewUserLoginHandler(
            IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public Task HandleAsync(EntityCreated<IdentityUserLogin> @event)
        {
            var login = @event.Entity;

            IJobDetail job = JobBuilder.Create<PullConnectionJob>()
                .WithIdentity(login.ProviderKey, login.LoginProvider)
                .UsingJobData("providerKey", login.ProviderKey)
                .UsingJobData("providerName", login.LoginProvider)
                .UsingJobData("ownerId", login.User.Id)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity(Guid.NewGuid().ToString(), login.LoginProvider)
              .StartAt(DateTimeOffset.Now.AddSeconds(5))
              .Build();

            _scheduler.ScheduleJob(job, trigger);

            return Task.FromResult(true);
        }
    }
}