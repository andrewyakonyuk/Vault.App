using System;

namespace Vault.Activity.Resources
{
    public class ArticleResource : ICanBeRead, ICanBeLiked
    {
        public Uri Uri { get; set; }
    }
}