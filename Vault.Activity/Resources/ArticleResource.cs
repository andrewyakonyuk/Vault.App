using System;

namespace Vault.Activity.Resources
{
    [Serializable]
    public class ArticleResource : ICanBeRead, ICanBeLiked
    {
        public Uri Uri { get; set; }
    }
}