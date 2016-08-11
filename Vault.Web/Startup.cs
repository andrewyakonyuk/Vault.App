using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Client;
using NEventStore.Dispatcher;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;
using Npgsql;
using Vault.Activity;
using Vault.Framework;
using Vault.Framework.Api.Boards.Overrides;
using Vault.Framework.Api.Users;
using Vault.Framework.Mvc;
using Vault.Framework.Mvc.Routing;
using Vault.Framework.Mvc.Routing.Constraints;
using Vault.Framework.Mvc.Routing.Projections;
using Vault.Shared.Connectors;
using Vault.Shared.Connectors.Pocket;
using Vault.Shared.Domain;
using Vault.Shared.EventSourcing;
using Vault.Shared.EventSourcing.NEventStore;
using Vault.Shared.Identity;
using Vault.Shared.Identity.Overrides;
using Vault.Shared.NHibernate;
using Vault.Shared.NHibernate.Conventions;
using Vault.Shared.Queries;

namespace Vault.Web
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.AddService(typeof(WorkContextAwareFilter));
            });

            services.AddTransient<WorkContextAwareFilter, WorkContextAwareFilter>();

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Cookies.ApplicationCookie.LoginPath = "/account/signIn";
                options.Cookies.ApplicationCookie.ExpireTimeSpan = new TimeSpan(0, 20, 0);
            })
            .AddUserStore<UserStore>()
            .AddRoleStore<RoleStore>()
            .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, EmptyMessageSender>();
            services.AddTransient<ISmsSender, EmptyMessageSender>();
            services.AddSingleton<IUrlHelper, DefaultUrlHelper>();

            services.AddSingleton<INHibernateInitializer, NHibernateInitializer>();
            services.AddTransient<IUnitOfWorkFactory, NHibernateUnitOfWorkFactory>();
            services.AddTransient<ILinqProvider, NHibernateLinqProvider>();
            services.AddSingleton<ISessionFactory>(x => x.GetRequiredService<INHibernateInitializer>()
                .GetConfiguration()
                .BuildSessionFactory());
            services.AddScoped<ISessionProvider, PerRequestSessionProvider>();

            services.AddVaultFramework();

            services.AddSingleton<Shared.ILogger, Shared.ConsoleLogger>();

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
            services.AddSingleton<IEventStoreInitializer, NEventStoreWithCustomPipelineFactory>();

            services.AddSingleton<IResourceKeyMapper, InMemoryResourceKeyMapper>();

            services.AddSingleton<IConfiguration>(s => Configuration);

            services.Configure<UrlOptions>(options =>
            {
                options.ServeCDNContent = !string.IsNullOrEmpty(Configuration["CDN_URL"]);
                options.CDNServerBaseUrl = Configuration["CDN_URL"];
            });

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddTransient<UsernameRouteConstraint>();

            services.Configure<PocketConnectionOptions>(options => options.ConsumerKey = Configuration["authentication:pocket:consumerKey"]);
            services.AddTransient<IPullConnectionProvider, PocketConnectionProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            Microsoft.Extensions.Logging.ILoggerFactory loggerFactory,
            IHostingEnvironment env,
            IConfiguration configuration)
        {
            app.UseStaticFiles();
            loggerFactory.AddConsole();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseRuntimeInfoPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            // Add cookie-based authentication to the request pipeline.
            app.UseIdentity();
            app.UsePocketAuthentication(options =>
            {
                options.ConsumerKey = Configuration["authentication:pocket:consumerKey"];
            });

            app.UseStatusCodePages();

            app.UseMvc(routes =>
            {
                //routes.DefaultHandler = new WorkContextAwareRoute(routes.DefaultHandler,
                //    routes.ServiceProvider.GetRequiredService<IWorkContextAccessor>(),
                //    routes.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>());

                routes.MapRoute(
                    name: "boards",
                    template: "{username:regex(^.+$)}/b/",
                    constraints: new
                    {
                        username = routes.ServiceProvider.GetRequiredService<UsernameRouteConstraint>()
                    },
                    defaults: new
                    {
                        controller = "Boards",
                        action = "Index"
                    }
                );

                routes.MapRoute(
                    name: "board-detail",
                    template: "{username:regex(^.+$)}/b/{title}-{boardId:int}",
                    constraints: new
                    {
                        username = routes.ServiceProvider.GetRequiredService<UsernameRouteConstraint>()
                    },
                    defaults: new
                    {
                        controller = "Boards",
                        action = "Detail"
                    },
                    projections: new
                    {
                        title = new DashedRouteProjection(false)
                    }
                );
                routes.MapRoute("board-search",
                    template: "{username:regex(^.+$)}/b/search",
                    constraints: new
                    {
                        username = routes.ServiceProvider.GetRequiredService<UsernameRouteConstraint>()
                    },
                    defaults: new
                    {
                        controller = "Boards",
                        action = "Search"
                    }
               );

                routes.MapRoute("user-profile",
                    template: "{username:regex(^.+$)}/a",
                    constraints: new
                    {
                        username = routes.ServiceProvider.GetRequiredService<UsernameRouteConstraint>()
                    },
                    defaults: new
                    {
                        controller = "Account",
                        action = "Index"
                    }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }

    public class NHibernateInitializer : INHibernateInitializer
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environmant;

        public NHibernateInitializer(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
            _environmant = environment;
        }

        public Configuration GetConfiguration()
        {
            var persistenceModel = AutoMap.AssemblyOf<IdentityUser>(new AutomappingConfiguration())
                .AddEntityAssembly(typeof(BoardMapping).Assembly)
                .UseOverridesFromAssemblyOf<NHibernateInitializer>()
                .UseOverridesFromAssemblyOf<BoardMapping>()
                .UseOverridesFromAssemblyOf<IdentityUserClaimOverride>()
                .Conventions.AddFromAssemblyOf<EntityMapConvention>();

            return Fluently
                .Configure()
                .CurrentSessionContext<ThreadStaticSessionContext>()
                    .Database(
                        PostgreSQLConfiguration.Standard
                        .ConnectionString(_configuration["connectionStrings:db"])
                        .AdoNetBatchSize(100).UseReflectionOptimizer()
                        .ShowSql())
                    .Mappings(m => m.AutoMappings.Add(persistenceModel))
                    .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(true, true))
                .BuildConfiguration();
        }
    }

    public class NEventStoreWithCustomPipelineFactory : IEventStoreInitializer
    {
        readonly IEnumerable<IPipelineHook> _pipelineHooks;
        readonly IConfiguration _configuration;

        public NEventStoreWithCustomPipelineFactory(
            IEnumerable<IPipelineHook> pipelineHooks,
            IConfiguration configuration)
        {
            _pipelineHooks = pipelineHooks;
            _configuration = configuration;
        }

        public IStoreEvents Create()
        {
            return Wireup
                .Init()
                    .LogToOutputWindow()
                    .UsingSqlPersistence(new PostgreSqlConnectionFactory(_configuration["connectionStrings:db"]))
                        .WithDialect(new PostgreSqlDialect())
                            .InitializeStorageEngine()
                    .UsingCustomSerialization(new NewtonsoftJsonSerializer(new VersionedEventSerializationBinder()))
                    // Compress Aggregate serialization. Does NOT allow to do a SQL-uncoding of varbinary Payload
                    // Comment if you need to decode message with CAST([Payload] AS VARCHAR(MAX)) AS [Payload] (on some VIEW)
                    //.Compress()
                    .UsingEventUpconversion()

                    //   .WithConvertersFromAssemblyContaining(new Type[] { typeof(ToDoEventsConverters) })
                    .HookIntoPipelineUsing(_pipelineHooks)
                    .Build();
        }
    }

    public class PostgreSqlConnectionFactory : IConnectionFactory
    {
        readonly string _connectionString;

        public PostgreSqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Type GetDbProviderFactoryType()
        {
            return typeof(NpgsqlFactory);
        }

        public IDbConnection Open()
        {
            return Open(_connectionString);
        }

        protected virtual IDbConnection Open(string connectionString)
        {
            var connection = NpgsqlFactory.Instance.CreateConnection();
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }
    }
}