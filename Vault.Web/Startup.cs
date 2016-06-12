using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Persistence.Sql.SqlDialects;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using Vault.Framework;
using Vault.Framework.Api.Boards.Overrides;
using Vault.Framework.Api.Users;
using Vault.Framework.Mvc;
using Vault.Framework.Mvc.Routing;
using Vault.Framework.Mvc.Routing.Projections;
using Vault.Shared.Connectors;
using Vault.Shared.Connectors.Pocket;
using Vault.Shared.Domain;
using Vault.Shared.Identity;
using Vault.Shared.Identity.Overrides;
using Vault.Shared.NEventStore;
using Vault.Shared.NHibernate;
using Vault.Shared.NHibernate.Conventions;

namespace Vault.Web
{
    public class Startup
    {
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
                options.Password.RequireNonLetterOrDigit = true;
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

            services.AddSingleton<IEventStoreInitializer, NEventStoreWithCustomPipelineFactory>();

            services.AddEventStore();
            services.AddVaultFramework();

            services.AddSingleton<Shared.ILogger, Shared.ConsoleLogger>();

            // Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1'
            // is found in both the registered sources, then the later source will win. By this way a Local config
            // can be overridden by a different setting while deployed remotely.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                //All environment variables in the process's context flow in as configuration values.
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            services.AddSingleton<IConfiguration>(s => configuration);

            services.Configure<UrlOptions>(options =>
            {
                options.ServeCDNContent = !string.IsNullOrEmpty(configuration["CDN_URL"]);
                options.CDNServerBaseUrl = configuration["CDN_URL"];
            });

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            services.Configure<PocketConnectionOptions>(options => options.ConsumerKey = configuration["authentication:pocket:consumerKey"]);
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
                options.ConsumerKey = configuration["authentication:pocket:consumerKey"];
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
                    defaults: new
                    {
                        controller = "Boards",
                        action = "Index"
                    }
                );

                routes.MapRoute(
                    name: "board-detail",
                    template: "{username:regex(^.+$)}/b/{title}-{boardId:int}",
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
                   defaults: new
                   {
                       controller = "Boards",
                       action = "Search"
                   }
               );

                routes.MapRoute("user-profile",
                    template: "{username:regex(^.+$)}",
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
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
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
                        MsSqlConfiguration.MsSql2012
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
                    .UsingSqlPersistence("EventStore", "System.Data.SqlClient", _configuration["connectionStrings:db"]) // Connection string is in web.config
                        .WithDialect(new MsSqlDialect())
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
}