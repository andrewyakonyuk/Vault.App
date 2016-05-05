namespace Vault.Framework.Models
{
    public class Article : CreativeWork
    {
        /// <summary>
        /// The actual body of the article.
        /// </summary>
        public virtual string Body { get; set; }

        /// <summary>
        /// The short description of the article
        /// </summary>
        public virtual string Summary { get; set; }

        public virtual Image Thumbnail { get; set; }
    }
}