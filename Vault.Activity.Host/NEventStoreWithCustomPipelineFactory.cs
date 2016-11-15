using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NEventStore;
using NEventStore.Persistence.Sql.SqlDialects;
using Vault.Shared.EventSourcing.NEventStore;

namespace Vault.Activity.Host
{
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
}