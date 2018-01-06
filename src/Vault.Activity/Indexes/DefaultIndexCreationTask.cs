using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared.Activity;
using Vault.Shared.Search;
using Vault.Shared.Search.Lucene;
using Vault.Shared.Search.Lucene.Converters;
using MultilineStringConverter = Vault.Shared.Search.Lucene.Converters.MultilineStringConverter;

namespace Vault.Activity.Indexes
{
    public class DefaultIndexCreationTask : AbstractIndexCreationTask<CommitedActivityEvent, dynamic>
    {
        public override string IndexName => IndexNames.Default;

        public DefaultIndexCreationTask()
        {
            Map = activities => from @event in activities
                                select new
                                {
                                    Id = @event.Id,
                                    Bucket = @event.Bucket,
                                    CheckpointToken = @event.CheckpointToken,
                                    StreamId = @event.StreamId,
                                    Actor = @event.Actor,
                                    Title = @event.Title,
                                    Content = @event.Content,
                                    Verb = @event.Verb,
                                    Target = @event.Target,
                                    Uri = @event.Uri,
                                    Provider = @event.Provider,
                                    Published = @event.Published.ToUniversalTime().DateTime
                                };
        }

        public override IndexDocumentMetadata GetIndexMetadata()
        {
            return new IndexMetadataBuilder()
                   .Field(nameof(CommitedActivityEvent.Id), "_id", isKey: true)
                   .Field(nameof(CommitedActivityEvent.Actor), isKey: true)
                   .Field(nameof(CommitedActivityEvent.Bucket), isKey: true)
                   .Field(nameof(CommitedActivityEvent.StreamId), "_ownerId", isKey: true)
                   .Field(nameof(CommitedActivityEvent.Provider), isKey: true)
                   .Field(nameof(CommitedActivityEvent.Verb), "_verb", isKey: true, isAnalysed: true)
                   .Field(nameof(CommitedActivityEvent.Published), "_published", converter: new LuceneDateTimeConverter())
                   .Field(nameof(CommitedActivityEvent.Title), isKeyword: true, isAnalysed: true)
                   .Field(nameof(CommitedActivityEvent.Content), isKeyword: true, isAnalysed: true)
                   .Field(nameof(CommitedActivityEvent.Target))
                   .Field(nameof(CommitedActivityEvent.CheckpointToken), converter: new Int64Converter())
                   .Field(nameof(CommitedActivityEvent.Uri), isAnalysed: true, isKeyword: true)
                   .Field("Tags", "_tags", converter: new MultilineStringConverter(), isKeyword: true, isAnalysed: true)
                   .NewMetadata();
        }
    }
}
