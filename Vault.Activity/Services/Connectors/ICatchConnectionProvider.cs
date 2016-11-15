using System.Threading.Tasks;

namespace Vault.Activity.Services.Connectors
{
    public interface ICatchConnectionProvider : IConnectionProvider
    {
        Task SubscribeAsync(SubscribeConnectionContext context);

        Task<CatchResult> CatchAsync(CatchConnectionContext context);

        Task UnsubscribeAsync(SubscribeConnectionContext context);
    }
}