using System;
using System.IO;
using System.Threading;
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
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;
using Orleans;
using Orleans.Runtime;
using Vault.Shared.Domain;
using Vault.Shared.Identity;
using Vault.Shared.Identity.Overrides;
using Vault.Shared.NHibernate;
using Vault.Shared.NHibernate.Conventions;
using Vault.WebHost.Mvc;
using Vault.WebHost.Mvc.Routing;
using Vault.WebHost.Mvc.Routing.Constraints;
using Vault.WebHost.Mvc.Routing.Projections;
using Vault.WebHost.Services;
using Vault.WebHost.Services.Boards;
using Vault.WebHost.Services.Boards.Overrides;
using Vault.WebHost.Services.Security;

namespace Vault.WebHost
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

            services.AddSingleton<IAuthorizationService, DefaultAuthorizationService>();
            services.AddScoped<IAuthorizer, DefaultAuthorizer>();
            services.AddScoped<WorkContext, WorkContext>();
            services.AddSingleton<IWorkContextAccessor, DefaultWorkContextAccessor>();

            services.AddTransient<IEmailSender, EmptyMessageSender>();
            services.AddTransient<ISmsSender, EmptyMessageSender>();

            services.AddScoped<IBoardsApi, BoardsApi>();

            services.AddSingleton<IUrlHelper, DefaultUrlHelper>();

            services.AddSingleton<INHibernateInitializer, NHibernateInitializer>();
            services.AddTransient<IUnitOfWorkFactory, NHibernateUnitOfWorkFactory>();
            services.AddTransient<ILinqProvider, NHibernateLinqProvider>();
            services.AddSingleton<ISessionFactory>(x => x.GetRequiredService<INHibernateInitializer>()
                .GetConfiguration()
                .BuildSessionFactory());
            services.AddScoped<ISessionProvider, PerRequestSessionProvider>();

            services.AddQueries();
            services.AddCommands();
            services.AddHandles();

            services.AddSingleton<Shared.ILogger, Shared.ConsoleLogger>();

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

            // Attempt to connect a few times to overcome transient failures and to give the silo enough
            // time to start up when starting at the same time as the client (useful when deploying or during development).
            const int initializeAttemptsBeforeFailing = 5;

            int attempt = 0;
            while (true)
            {
                try
                {
                    GrainClient.Initialize("ClientConfiguration.xml");
                    break;
                }
                catch (SiloUnavailableException)
                {
                    attempt++;
                    if (attempt >= initializeAttemptsBeforeFailing)
                    {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
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
}