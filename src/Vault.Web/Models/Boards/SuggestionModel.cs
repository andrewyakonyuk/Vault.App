using Vault.WebHost.Services.Boards;

namespace Vault.WebHost.Models.Boards
{
    public class SuggestionModel
    {
        public string Value { get; set; }
        public string Type { get; set; }
        public Card Card { get; set; }
    }
}