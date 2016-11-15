using System;
using System.Collections.Generic;
using Vault.Activity.Resources;
using Vault.Shared.EventSourcing;
using Vault.Shared.EventSourcing.NEventStore;

namespace Vault.Activity.Services.ActivityLog
{
    public class ActivityLog : AggregateRootBase
    {
        readonly Dictionary<ResourceKey, Article> _articles;

        protected ActivityLog(Guid id)
        {
            Id = id;
            _articles = new Dictionary<ResourceKey, Article>();
        }

        public Article ForArticle(ResourceKey key)
        {
            Article article;
            if (!_articles.TryGetValue(key, out article))
            {
                article = new Article(key, this);
                _articles.Add(key, article);
                ((IRegisterChildEntities)this).RegisterChildEventProvider(article);
            }

            return article;
        }

        void Apply(IHasResource<ArticleResource> @event)
        {
            var article = ForArticle(@event.ItemKey);
            ((IEventProvider)article).LoadFromHistory(new[] { (IEvent)@event });
        }
    }
}