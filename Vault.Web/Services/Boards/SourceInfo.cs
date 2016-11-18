using Vault.Shared.Domain;

namespace Vault.WebHost.Services.Boards
{
    public class SourceInfo : IEntityComponent
    {
        public string Url { get; set; }

        public string Provider { get; set; }
    }
}