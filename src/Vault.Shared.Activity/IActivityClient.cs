using System;
using System.Threading.Tasks;

namespace Vault.Shared.Activity
{
    public interface IActivityClient
    {
        Task<IActivityStream> GetStreamAsync(string bucket, string streamId);
    }
}
