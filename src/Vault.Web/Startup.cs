using System;
using System.IO;
using System.Threading;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;
using Vault.Shared.Domain;
using Vault.WebApp.Infrastructure.Identity;
using Vault.Shared.NHibernate;
using Vault.Shared.NHibernate.Conventions;
using Vault.WebApp.Services;
using Vault.WebApp.Services.Boards;
using Vault.WebApp.Services.Boards.Overrides;
using Vault.WebApp.Services.Security;
using Vault.Shared.Authentication.Pocket;
using System.Threading.Tasks;
using Vault.WebApp.Infrastructure.Mvc;
using Vault.WebApp.Infrastructure.Mvc.Routing;
using Vault.WebApp.Infrastructure.Mvc.Routing.Constraints;
using Vault.WebApp.Infrastructure.Mvc.Routing.Projections;
using Vault.WebApp.Infrastructure.Authentication.Cookies;
using Vault.WebApp.Infrastructure.Persistence;
using StreamInsights;
using Vault.WebApp.Infrastructure.Spouts;
using Vault.Spouts.Pocket;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using StreamInsights.Abstractions;
using StreamInsights.Persistance;
using MediatR;
using Vault.WebApp.Infrastructure.MediatR;

namespace Vault.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        IApplicationBuilder _app;
        CancellationTokenSource _environmentTokenSource;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _environmentTokenSource = new CancellationTokenSource();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.AddService(typeof(WorkContextAuthorizationFilter));
            });

            services.AddTransient<WorkContextAuthorizationFilter, WorkContextAuthorizationFilter>();

            //authentication and authorization
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie()
                .AddTwitter(options => { options.ConsumerKey = Configuration["authentication:pocket:consumerKey"]; });

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddUserStore<UserStore>()
            .AddRoleStore<RoleStore>()
            .AddDefaultTokenProviders();

            services.AddStreamInsights(options=>
            {
                options.ConnectionString = Configuration["connectionStrings:db"];
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login";
                options.Events = new ExtendedCookieAuthenticationEvents();
            });

            services.AddMediatR();

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

            services.AddSingleton<IConfiguration>(s => Configuration);

            services.Configure<UrlOptions>(options =>
            {
                options.ServeCDNContent = !string.IsNullOrEmpty(Configuration["CDN_URL"]);
                options.CDNServerBaseUrl = Configuration["CDN_URL"];
            });

            services.Configure<SqlConnectionFactoryOptions>(options =>
            {
                options.ConnectionString = Configuration["connectionStrings:db"];
            });

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddTransient<UsernameRouteConstraint>();
            services.AddTransient<UsernameRouteProjection>();

            services.AddSingleton<IDbConnectionFactory, Infrastructure.Persistence.PostgreSqlConnectionFactory>();

            services.Configure<SpoutOptions>(options =>
            {
                options.Services.Add("pocket", typeof(PocketSpout));
            });

            services.Configure<PocketSpoutOptions>(options =>
            {
                options.ConsumerKey = Configuration["authentication:pocket:consumerKey"];
            });

            services.AddSingleton<SpoutManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            Microsoft.Extensions.Logging.ILoggerFactory loggerFactory,
            IHostingEnvironment env,
            IConfiguration configuration,
            IApplicationLifetime applicationLifetime)
        {
            _app = app;

            app.UseStaticFiles();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            // Add cookie-based authentication to the request pipeline.
            app.UseAuthentication();

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
                    projections: new
                    {
                        username = routes.ServiceProvider.GetRequiredService<UsernameRouteProjection>()
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
                    projections: new
                    {
                        title = new DashedRouteProjection(false),
                        username = routes.ServiceProvider.GetRequiredService<UsernameRouteProjection>()
                    },
                    defaults: new
                    {
                        controller = "Boards",
                        action = "Detail"
                    }
                );
                routes.MapRoute("board-search",
                    template: "{username:regex(^.+$)}/b/search",
                    constraints: new
                    {
                        username = routes.ServiceProvider.GetRequiredService<UsernameRouteConstraint>()
                    },
                    projections: new
                    {
                        username = routes.ServiceProvider.GetRequiredService<UsernameRouteProjection>()
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
                    projections: new
                    {
                        username = routes.ServiceProvider.GetRequiredService<UsernameRouteProjection>()
                    },
                    defaults: new
                    {
                        controller = "Account",
                        action = "Index"
                    }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Boards}/{action=Index}/{id?}");
            });

            applicationLifetime.ApplicationStarted.Register(OnApplicationStarted);
            applicationLifetime.ApplicationStopping.Register(OnApplicationStopping);
        }

        void OnApplicationStarted()
        {
            var appendStore = _app.ApplicationServices.GetRequiredService<IAppendOnlyActivityStore>();
            var mediator = _app.ApplicationServices.GetRequiredService<IMediator>();

            var subscription = new PollingSubscription<CommitedActivity>(appendStore.ReadAsync,
                (activity, token) => mediator.Publish(Notification.Create(activity), token), 
                cancellationToken: _environmentTokenSource.Token);

            subscription.Run();
        }

        void OnApplicationStopping()
        {
            _environmentTokenSource.Dispose();
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
            var persistenceModel = AutoMap.AssemblyOf<BoardMapping>(new AutomappingConfiguration())
                .UseOverridesFromAssemblyOf<NHibernateInitializer>()
                .UseOverridesFromAssemblyOf<BoardMapping>()
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

    public class CommitedActivityHandler : INotificationHandler<Notification<CommitedActivity>>
    {
        public Task Handle(Notification<CommitedActivity> notification, CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }
    }
}