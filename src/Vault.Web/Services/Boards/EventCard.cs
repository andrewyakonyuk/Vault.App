using System;

namespace Vault.WebApp.Services.Boards
{
    public class EventCard : Card
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan Duration { get; set; }
    }
}