using System.Threading.Tasks;

namespace Vault.Activity.Services.Connectors
{
    public interface IPullConnectionProvider : IConnectionProvider
    {
        Task<PullResult> PullAsync(PullConnectionContext context);
    }
}