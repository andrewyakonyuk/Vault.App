using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Vault.Framework.Api.Boards;
using Vault.Framework.Search;
using Vault.Framework.Search.Parsing;
using Vault.Framework.Security;
using Vault.Shared.Commands;
using Vault.Shared.Events;
using Vault.Shared.Lucene;
using Vault.Shared.Lucene.Analyzers;
using Vault.Shared.Lucene.Converters;
using Vault.Shared.Queries;

namespace Vault.Framework
{
    public static class ServiceCollectionExtensions
    {
        public static void AddVaultFramework(this IServiceCollection services)
        {
            services.AddQueries();
            services.AddCommands();
            services.AddHandles();

            services.AddSingleton<IAuthorizationService, DefaultAuthorizationService>();
            services.AddScoped<IAuthorizer, DefaultAuthorizer>();
            services.AddScoped<WorkContext, WorkContext>();
            services.AddSingleton<IWorkContextAccessor, DefaultWorkContextAccessor>();
            services.AddSingleton<IEventPublisher, ImMemoryEventPublisher>();

            services.AddSearch();

            services.AddScoped<IBoardsApi, BoardsApi>();
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
                .Field("Summary", isAnalysed: true);

            services.AddSingleton<IIndexDocumentMetadataProvider>(s => builder.Build());
            services.AddTransient<ISearchQueryParser, DefaultSearchQueryParser>();
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
                    handler.Handle(@event);
                }
            }
        }
    }

    internal class SearchDocumentGenerator : IHandle<EntityCreated<Board>>
    {
        readonly IWorkContextAccessor _workContextAccessor;
        readonly IReportUnitOfWorkFactory _reportUnitOfWorkFactory;

        public SearchDocumentGenerator(
            IWorkContextAccessor workContextAccessor,
            IReportUnitOfWorkFactory reportUnitOfWorkFactory)
        {
            _workContextAccessor = workContextAccessor;
            _reportUnitOfWorkFactory = reportUnitOfWorkFactory;
        }

        public void Handle(EntityCreated<Board> @event)
        {
            Hook();
        }

        private void Hook()
        {
            var random = new Random();

            var types = new[] { "Event", "Place", "Article", "Audio" };
            var mapPoints = new[] {
                new {
                    Latitude = 40.714728, Longitude = -73.998672
                },
                 new {
                    Latitude = 49.24195, Longitude = 8.5491213
                },
                  new {
                    Latitude = 50.4496346, Longitude = 30.5231952
                },
                   new {
                    Latitude = 50.2481061, Longitude = 28.6802412
                },
            };

            using (var unitOfWork = _reportUnitOfWorkFactory.Create())
            {
                for (int i = _workContextAccessor.WorkContext.Owner.Id * 1001; i < _workContextAccessor.WorkContext.Owner.Id * 1001 + 10000; i++)
                {
                    var actualType = types[Math.Min((int)(random.Next(0, 40) / 10), 3)];

                    dynamic searchDocument = new SearchDocument();

                    searchDocument.Id = i;
                    searchDocument.OwnerId = _workContextAccessor.WorkContext.Owner.Id;
                    searchDocument.Published = DateTime.UtcNow.AddDays(i);
                    searchDocument.DocumentType = actualType;

                    if (actualType == "Event")
                    {
                        searchDocument.Name = "Event" + i;
                        searchDocument.Description = "EventDescription" + i;
                        searchDocument.Duration = new TimeSpan(1, 30, 0);
                        searchDocument.StartDate = DateTime.UtcNow;
                        searchDocument.EndDate = DateTime.UtcNow.AddHours(1.5);
                    }
                    else if (actualType == "Place")
                    {
                        searchDocument.Name = "Place" + i;
                        searchDocument.Description = "Place description" + i;
                        searchDocument.Elevation = random.NextDouble();
                        var point = mapPoints[Math.Min((int)(random.Next(0, 40) / 10), 3)];
                        searchDocument.Latitude = point.Latitude;
                        searchDocument.Longitude = point.Longitude;
                    }
                    else if (actualType == "Audio")
                    {
                        searchDocument.Name = "Audio" + i;
                        searchDocument.Description = "Audio description" + i;
                        searchDocument.ByArtist = "By artist" + i;
                        searchDocument.InAlbum = "In album" + i;
                        searchDocument.Duration = new TimeSpan(0, 3, 33);
                    }
                    else if (actualType == "Article")
                    {
                        searchDocument.Name = "Article" + i;
                        searchDocument.Description = "Article description" + i;
                        searchDocument.Body = "Article body" + i;
                        searchDocument.Summary = "Article summary" + i;
                    }

                    unitOfWork.Save(searchDocument);
                }

                unitOfWork.Commit();
            }
        }
    }
}