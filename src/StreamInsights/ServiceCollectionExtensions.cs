using Dapper;
using Microsoft.Extensions.DependencyInjection;
using StreamInsights.Abstractions;
using StreamInsights.Persistance;
using StreamInsights.Persistance.TypeHandlers;
using System;

namespace StreamInsights
{
    public static class ServiceCollectionExtensions
    {
        public static void AddStreamInsights(this IServiceCollection services, Action<StreamInsightsOptions> configure)
        {
            var options = new StreamInsightsOptions();
            configure(options);

            services.AddSingleton<IClock, DefaultClock>();
            services.AddSingleton<IActivityClient, DefaultActivityClient>();

            services.AddTransient<ISqlConnectionFactory, PostgreSqlConnectionFactory>(_ => new PostgreSqlConnectionFactory(options.ConnectionString));
            services.AddSingleton<IAppendOnlyActivityStore, SqlAppendOnlyActivityStore>();
            
            SqlMapper.AddTypeHandler(JObjectHandler.Default);
            SqlMapper.AddTypeHandler(StringValuesHandler.Default);
            SqlMapper.AddTypeHandler(ASObjectValuesHandler.Default);
            SqlMapper.AddTypeHandler(StringDictionaryHandler.Default);
        }
    }

    public class StreamInsightsOptions
    {
        public string ConnectionString { get; set; }
    }
}
