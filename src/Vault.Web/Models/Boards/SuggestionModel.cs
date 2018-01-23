using Vault.WebApp.Services.Boards;

namespace Vault.WebApp.Models.Boards
{
    public class SuggestionModel
    {
        public string Value { get; set; }
        public string Type { get; set; }
        public Card Card { get; set; }
    }
}