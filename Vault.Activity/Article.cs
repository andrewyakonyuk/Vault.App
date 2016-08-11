using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using CommonDomain.Core;
using Vault.Activity.Events;
using Vault.Activity.Resources;

namespace Vault.Activity
{
    public class Article : AggregateBase
    {
        readonly HashSet<Uri> _urls;

        protected Article(IRouteEvents handler)
            : base(handler)
        {
            _urls = new HashSet<Uri>();
        }

        protected Article(Guid id)
        {
            _urls = new HashSet<Uri>();
            Id = id;
        }

        public Article(
            ResourceKey key)
            : this(Guid.NewGuid())
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
                RaiseEvent(new LikedActivityEvent<ArticleResource>(Id, ItemKey, published));
        }

        public void Dislike(DateTimeOffset published)
        {
            if (Liked)
                RaiseEvent(new DislikedActivityEvent<ArticleResource>(Id, ItemKey, published));
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