using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Activity.Sinks;
using Vault.Shared.Search;

namespace Vault.Activity.Indexes
{
    public class IndexBatchingAdapter<TDocument> : IPeriodicBatchingAdapter<TDocument>
    {
        readonly IIndexUnitOfWorkFactory _unitOfWorkFactory;
        readonly IEnumerable<AbstractIndexCreationTask<TDocument>> _indexCreationTasks;

        public IndexBatchingAdapter(
            IIndexUnitOfWorkFactory unitOfWorkFactory,
            IEnumerable<AbstractIndexCreationTask<TDocument>> indexCreationTasks)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _indexCreationTasks = indexCreationTasks;
        }

        public int BatchSizeLimit => 100;

        public TimeSpan Period => TimeSpan.FromSeconds(3);

        public int? QueueLimit => 1000;

        public bool CanInclude(TDocument message)
        {
            return true;
        }

        public Task EmitBatchAsync(IEnumerable<TDocument> messages)
        {
            foreach (var indexCreation in _indexCreationTasks)
            {
                var definition = indexCreation.GetIndexMetadata();
                //todo: include here the task of creating an indexinvolve index creation task
                using (var uow = _unitOfWorkFactory.Create(indexCreation.IndexName))
                {
                    foreach (var @event in messages.OfType<CommitedActivityEvent>())
                    {
                        dynamic document = new SearchDocument();
                        document.Id = @event.Id;
                        document.Bucket = @event.Bucket;
                        document.CheckpointToken = @event.CheckpointToken;
                        document.StreamId = @event.StreamId.ToString("N");
                        document.Actor = @event.Actor;
                        document.Title = @event.Title;
                        document.Content = @event.Content;
                        document.Verb = @event.Verb;
                        document.Target = @event.Target;
                        document.Uri = @event.Uri;
                        document.Provider = @event.Provider;
                        document.Published = @event.Published.ToUniversalTime().DateTime;
                        document.Tags = @event.MetaBag.Tags;
                        document.Thumbnail = @event.MetaBag.Thumbnail;
                        //todo: copy from meta

                        uow.Save(document);
                    }
                    uow.Commit();
                }
            }

            return Task.FromResult(true);
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.FromResult(true);
        }

        public void Dispose()
        {

        }
    }
}
