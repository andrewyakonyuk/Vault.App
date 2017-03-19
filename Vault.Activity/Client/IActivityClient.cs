using System;
using System.Threading.Tasks;

namespace Vault.Activity.Client
{
    public interface IActivityClient
    {
        Task<IActivityFeed> GetFeedAsync(string bucket, Guid streamId);
    }
}
