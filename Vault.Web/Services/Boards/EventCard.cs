using System;

namespace Vault.WebHost.Services.Boards
{
    public class EventCard : Card
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan Duration { get; set; }
    }
}