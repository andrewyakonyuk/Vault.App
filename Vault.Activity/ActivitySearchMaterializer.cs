using Orleans;
using Orleans.Streams;
using System.Threading.Tasks;
using Vault.Shared.Search;

namespace Vault.Activity
{
    public interface IActivitySearchMaterializer : IGrainWithGuidKey
    {
    }

    [ImplicitStreamSubscription("activity-log")]
    public class ActivitySearchMaterializer : Grain, IActivitySearchMaterializer
    {
        private readonly IIndexUnitOfWorkFactory _unitOfWorkFactory;

        public ActivitySearchMaterializer(IIndexUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            var streamProvider = GrainClient.GetStreamProvider("EventStream");
            var consumer = streamProvider.GetStream<ActivityEvent>(this.GetPrimaryKey(), "activity-log");
            await consumer.SubscribeAsync((@event, token) => NewActivityAsync(@event, token));
        }

        public Task NewActivityAsync(ActivityEvent @event, StreamSequenceToken token)
        {
            using (var uow = _unitOfWorkFactory.Create(IndexNames.Default))
            {
                dynamic document = new SearchDocument();
                document.Id = @event.Id;
                document.OwnerId = @event.Actor;
                document.Title = @event.Title;
                document.Content = @event.Content;
                document.Verb = @event.Verb;
                document.Target = @event.Target;
                document.Url = @event.Uri;
                document.Provider = @event.Provider;
                document.Published = @event.Published.ToUniversalTime().DateTime;
                //todo: copy from meta

                uow.Save(document);
                uow.Commit();
            }

            return TaskDone.Done;
        }
    }
}