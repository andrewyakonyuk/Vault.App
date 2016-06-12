using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Vault.Domain.Activities;
using Vault.Framework.Api.Boards;
using Vault.Framework.Search.Parsing;
using Vault.Framework.Security;
using Vault.Shared.Commands;
using Vault.Shared.Connectors;
using Vault.Shared.Events;
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
            services.AddTransient<IIndexWriterInitializer, InMemoryWriterInitializer>();
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
                .Field("Elevation", converter: new DecimalConverter())
                .Field("Latitude", converter: new DecimalConverter())
                .Field("Longitude", converter: new DecimalConverter())
                .Field("ByArtist", isKeyword: true, isAnalysed: true)
                .Field("InAlbum", isKeyword: true, isAnalysed: true)
                .Field("Body", isAnalysed: true)
                .Field("Summary", isAnalysed: true)
                .Field("Thumbnail")
                .Field("Url");

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

    internal class InMemoryWriterInitializer : IIndexWriterInitializer
    {
        public LuceneIndexWriter Create()
        {
            var directory = FSDirectory.Open(Path.Combine(Environment.CurrentDirectory, "index"));
            var shouldCreate = !directory.Directory.Exists || !directory.ListAll().Any();
            var writer = new LuceneIndexWriter(directory, new LowerCaseAnalyzer(Lucene.Net.Util.Version.LUCENE_30), shouldCreate, IndexWriter.MaxFieldLength.UNLIMITED);

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

    public class NewBoardHandler : IHandle<EntityCreated<Board>>
    {
        IScheduler _scheduler;
        IQueryBuilder _queryBuilder;

        public NewBoardHandler(
            IScheduler scheduler,
            IQueryBuilder queryBuilder)
        {
            _scheduler = scheduler;
            _queryBuilder = queryBuilder;
        }

        public async Task HandleAsync(EntityCreated<Board> @event)
        {
            var user = await _queryBuilder.For<IdentityUser>().ById(@event.Entity.OwnerId);

            foreach (var item in user.Logins)
            {
                IJobDetail job = JobBuilder.Create<PullConnectionJob>()
                    .WithIdentity(item.ProviderKey, item.LoginProvider)
                    .UsingJobData("providerKey", item.ProviderKey)
                    .UsingJobData("providerName", item.LoginProvider)
                    .UsingJobData("ownerId", item.User.Id)
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                  .WithIdentity(Guid.NewGuid().ToString(), item.LoginProvider)
                  .StartAt(DateTimeOffset.Now.AddSeconds(5))
                  .Build();

                _scheduler.ScheduleJob(job, trigger);
            }
        }
    }

    public class ReadActivityHandler : IHandle<ReadActivity>
    {
        readonly IReportUnitOfWorkFactory _reportUnitOfWorkFactory;
        static Random random = new Random();

        public ReadActivityHandler(IReportUnitOfWorkFactory reportUnitOfWorkFactory)
        {
            _reportUnitOfWorkFactory = reportUnitOfWorkFactory;
        }

        public Task HandleAsync(ReadActivity @event)
        {
            using (var unitOfWork = _reportUnitOfWorkFactory.Create())
            {
                dynamic searchDocument = new SearchDocument();

                searchDocument.Id = random.Next(int.MaxValue);
                searchDocument.OwnerId = @event.OwnerId;
                searchDocument.Published = DateTime.UtcNow;
                searchDocument.DocumentType = "Article";
                searchDocument.Url = @event.Affected.Url;

                searchDocument.Name = @event.Affected.Name;
                searchDocument.Description = @event.Affected.Description;
                searchDocument.Body = @event.Affected.Description;
                searchDocument.Summary = @event.Affected.Description;
                if (@event.Affected.Image != null)
                    searchDocument.Thumbnail = @event.Affected.Image.Url;

                unitOfWork.Save(searchDocument);
                unitOfWork.Commit();
            }

            return Task.FromResult(true);
        }
    }
}