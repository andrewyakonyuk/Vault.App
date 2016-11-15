using System.Threading.Tasks;
using Orleans;
using Orleans.Providers;

namespace Vault.Activity.Host
{
    public class Bootstrap : IBootstrapProvider
    {
        public string Name => "Activity";

        public Task Close()
        {
            return TaskDone.Done;
        }

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            return TaskDone.Done;
        }
    }
}