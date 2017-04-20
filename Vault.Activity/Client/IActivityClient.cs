using System;
using System.Threading.Tasks;

namespace Vault.Activity.Client
{
    public interface IActivityClient
    {
        Task<IActivityStream> GetStreamAsync(string bucket, string streamId);
    }
}
