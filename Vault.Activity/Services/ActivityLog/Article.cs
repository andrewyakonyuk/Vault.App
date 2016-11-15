using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using CommonDomain.Core;
using Vault.Activity.Events;
using Vault.Activity.Resources;
using Vault.Shared.EventSourcing.NEventStore;

namespace Vault.Activity.Services.ActivityLog
{
    public class Article : EntityBase
    {
        readonly HashSet<Uri> _urls;

        protected Article(Guid id, AggregateRootBase aggregate)
            : base(aggregate)
        {
            _urls = new HashSet<Uri>();
            Id = id;
        }

        public Article(
            ResourceKey key, AggregateRootBase aggregate)
            : this(Guid.NewGuid(), aggregate)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            ItemKey = key;
        }

        public ResourceKey ItemKey { get; private set; }

        public bool Liked { get; private set; }

        public IReadOnlyList<Uri> Urls { get { return _urls.ToList().AsReadOnly(); } }

        public void Read(Uri uri, DateTimeOffset published)
        {
            if (_urls.Add(uri))
            {
                var article = new ArticleResource { Uri = uri };
                RaiseEvent(new ReadActivityEvent<ArticleResource>(Id, article, ItemKey, published));
            }
        }

        public void Like(DateTimeOffset published)
        {
            if (!Liked)
                RaiseEvent(new LikedActivityEvent<ArticleResource>(Id, default(ArticleResource), ItemKey, published));
        }

        public void Dislike(DateTimeOffset published)
        {
            if (Liked)
                RaiseEvent(new DislikedActivityEvent<ArticleResource>(Id, default(ArticleResource), ItemKey, published));
        }

        void Apply(ReadActivityEvent<ArticleResource> e)
        {
            ItemKey = e.ItemKey;
            _urls.Add(e.Resource.Uri);
        }

        void Apply(LikedActivityEvent<ArticleResource> e)
        {
            Liked = true;
            ItemKey = e.ItemKey;
        }

        void Apply(DislikedActivityEvent<ArticleResource> e)
        {
            Liked = false;
            ItemKey = e.ItemKey;
        }
    }
}