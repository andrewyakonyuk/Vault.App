using System;

namespace Vault.Framework.Api.Boards
{
    public class ArticleCard : Card
    {
        public string Summary { get; set; }
        public string Body { get; set; }
        public Uri Url { get; set; }
    }
}