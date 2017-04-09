using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Vault.Shared.Queries;

namespace Vault.Activity.Host
{
    public static class ServiceCollectionExtensions
    {
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
            return query;
        }
    }
}