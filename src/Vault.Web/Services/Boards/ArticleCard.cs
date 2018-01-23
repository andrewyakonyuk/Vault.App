using System;

namespace Vault.WebApp.Services.Boards
{
    public class ArticleCard : Card
    {
        public string Summary { get; set; }
        public string Body { get; set; }
        public string Url { get; set; }
        public string Thumbnail { get; set; }
    }
}