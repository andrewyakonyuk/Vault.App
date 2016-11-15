using System;
using System.Threading.Tasks;
using CommonDomain.Persistence;
using Orleans;
using Orleans.Streams;
using Vault.Activity.Commands;
using Vault.Activity.Events;
using Vault.Activity.Resources;

namespace Vault.Activity.Services.ActivityLog
{
    public interface IActivityLogWorker : IGrainWithGuidKey
    {
    }

    [ImplicitStreamSubscription(PushActivityNotifier.CommandStreamNamespace)]
    public class ActivityLogWorker : Grain, IActivityLogWorker
    {
        const string StreamNamespace = "activity-log";
        ActivityLog _activityLog;
        readonly IRepository _repository;

        public ActivityLogWorker(IRepository repository)
        {
            _repository = repository;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            var streamProvider = GetStreamProvider("ActivityProvider");
            var consumer = streamProvider.GetStream<ActivityCommandBase>(this.GetPrimaryKey(), PushActivityNotifier.CommandStreamNamespace);
            await consumer.SubscribeAsync(OnNextAsync, OnErrorAsync, OnCompletedAsync);

            _activityLog = _repository.GetById<ActivityLog>(StreamNamespace, this.GetPrimaryKey());
        }

        public Task OnCompletedAsync()
        {
            _repository.Save(StreamNamespace, _activityLog, Guid.NewGuid());
            return TaskDone.Done;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return TaskDone.Done;
        }

        public Task OnNextAsync(ActivityCommandBase item, StreamSequenceToken token = null)
        {
            return OnNextActivityAsync((dynamic)item);
        }

        public Task OnNextActivityAsync(ReadActivityCommand<ArticleResource> c)
        {
            var article = _activityLog.ForArticle(c.ItemKey);
            article.Read(c.Resource.Uri, c.Published);

            return TaskDone.Done;
        }

        public Task OnNextActivityAsync(LikeActivityCommand<ArticleResource> c)
        {
            var article = _activityLog.ForArticle(c.ItemKey);
            article.Like(c.Published);

            return TaskDone.Done;
        }

        public Task OnNextActivityAsync(DislikeActivityCommand<ArticleResource> c)
        {
            var article = _activityLog.ForArticle(c.ItemKey);
            article.Dislike(c.Published);

            return TaskDone.Done;
        }

        public Task OnNextActivityAsync(ActivityCommandBase c)
        {
            // fallback
            return TaskDone.Done;
        }
    }
}