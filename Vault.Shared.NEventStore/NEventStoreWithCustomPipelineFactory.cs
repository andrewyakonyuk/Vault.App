using NEventStore;
using System.Collections.Generic;

namespace Vault.Shared.NEventStore
{
    public class NEventStoreWithCustomPipelineFactory : IEventStoreInitializer
    {
        readonly IEnumerable<IPipelineHook> _pipelineHooks;

        public NEventStoreWithCustomPipelineFactory(IEnumerable<IPipelineHook> pipelineHooks)
        {
            _pipelineHooks = pipelineHooks;
        }

        public IStoreEvents Create()
        {
            return Wireup
                .Init()
                    .LogToOutputWindow()
                    .UsingInMemoryPersistence()
                    .InitializeStorageEngine()

                    //.UsingSqlPersistence("EventStore") // Connection string is in web.config
                    //    .WithDialect(new MsSqlDialect())
                    //        .InitializeStorageEngine()
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