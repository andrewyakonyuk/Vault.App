using System;

namespace Vault.WebApp.Services.Boards
{
    public class AudioCard : Card
    {
        public TimeSpan Duration { get; set; }

        public string Thumbnail { get; set; }

        public string ByArtist { get; set; }

        public string InAlbum { get; set; }
    }
}