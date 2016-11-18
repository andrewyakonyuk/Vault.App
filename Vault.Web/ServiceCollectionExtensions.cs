using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Vault.Shared.Commands;
using Vault.Shared.EventSourcing;
using Vault.Shared.Queries;

namespace Vault.WebHost
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
}