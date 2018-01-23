using Vault.Shared.Domain;

namespace Vault.WebApp.Services.Boards
{
    public class SourceInfo : IEntityComponent
    {
        public string Url { get; set; }

        public string Provider { get; set; }
    }
}