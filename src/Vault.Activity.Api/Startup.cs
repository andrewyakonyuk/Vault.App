using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vault.Activity.Services.Connectors;
using Vault.Activity.Sinks;
using Vault.Shared.Search;
using Vault.Shared.Search.Lucene;
using Vault.Shared.Search.Parsing;
using Vault.Activity.Client;
using Vault.Activity.Persistence;
using Vault.Activity.Utility;
using Vault.Activity.Indexes;
using Vault.Activity.Api.Mvc;
using Vault.Shared.Activity;

namespace Vault.Activity.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options=>
            {
                options.Filters.Add(typeof(ValidateActionFilterAttribute));
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info()
                {
                    Title = "Vault.Activity HTTP API",
                    Version = "v1",
                    Description = "The Vault.Activity HTTP API",
                    TermsOfService = "Terms Of Service"
                });
            });

            services.AddSingleton<IConfiguration>(Configuration);

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
            services.AddSingleton<ISqlConnectionFactory, PostgreSqlConnectionFactory>(_ => new PostgreSqlConnectionFactory(Configuration["connectionStrings:db"]));

            services.AddSingleton<ISink<UncommitedActivityEvent>, PluggableBatchingSink<UncommitedActivityEvent>>(s =>
            {
                var store = s.GetRequiredService<IAppendOnlyStore>();
                var adapter = new AppendOnlyStoreBatchingAdapter(store);

                var clock = s.GetRequiredService<IClock>();
                var loggerFactory = s.GetRequiredService<ILoggerFactory>();
                return new PluggableBatchingSink<UncommitedActivityEvent>(adapter, loggerFactory, clock);
            });
            services.AddSingleton<ISink<CommitedActivityEvent>, PluggableBatchingSink<CommitedActivityEvent>>(s =>
            {
                var indexAccessor = s.GetRequiredService<IIndexStoreAccessor>();
                var indexTasks = s.GetServices<AbstractIndexCreationTask<CommitedActivityEvent>>();

                var adapter = new IndexBatchingAdapter<CommitedActivityEvent>(indexAccessor, indexTasks);
                var clock = s.GetRequiredService<IClock>();
                var loggerFactory = s.GetRequiredService<ILoggerFactory>();
                return new PluggableBatchingSink<CommitedActivityEvent>(adapter, loggerFactory, clock);
            });
            services.AddSingleton<IActivityClient, DefaultActivityClient>();
            services.AddSingleton<JsonSerializer>(_ => new JsonSerializer());
            services.AddSingleton<AbstractIndexCreationTask<CommitedActivityEvent>, DefaultIndexCreationTask>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}
