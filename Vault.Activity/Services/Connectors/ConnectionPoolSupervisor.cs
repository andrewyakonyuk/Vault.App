using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans;

namespace Vault.Activity.Services.Connectors
{
    public interface IConnectionPoolSupervisor : IGrainWithGuidKey
    {
        Task ConnectLoginAsync(string providerName, string providerKey);

        Task DisconnectLoginAsync(string providerName, string providerKey);

        Task<IEnumerable<ConnectionKey>> GetLoginsAsync();
    }

    [Serializable]
    public class ConnectionPoolState
    {
        public ConnectionPoolState()
        {
            Logins = new Dictionary<ConnectionKey, Guid>();
        }

        public Guid OwnerId { get; set; }

        public Dictionary<ConnectionKey, Guid> Logins { get; protected set; }
    }

    public class ConnectionPoolSupervisor : Grain<ConnectionPoolState>, IConnectionPoolSupervisor
    {
        public async Task ConnectLoginAsync(string providerName, string providerKey)
        {
            var connectionKey = new ConnectionKey(this.GetPrimaryKey(), providerName, providerKey);
            if (!State.Logins.ContainsKey(connectionKey))
            {
                var internalKey = Guid.NewGuid();
                var catchConnection = GrainFactory.GetGrain<ICatchConnectionWorker>(internalKey);
                await catchConnection.TryConnectAsync(connectionKey);

                var pollingConnection = GrainFactory.GetGrain<IPollingConnectionWorker>(internalKey);
                await pollingConnection.TryConnectAsync(connectionKey);

                State.Logins.Add(connectionKey, internalKey);
                await WriteStateAsync();
            }
        }

        public async Task DisconnectLoginAsync(string providerName, string providerKey)
        {
            var connectionKey = new ConnectionKey(this.GetPrimaryKey(), providerName, providerKey);
            Guid internalKey;
            if (State.Logins.TryGetValue(connectionKey, out internalKey))
            {
                var catchConnection = GrainFactory.GetGrain<ICatchConnectionWorker>(internalKey);
                await catchConnection.DisconnectAsync();

                var pollingConnection = GrainFactory.GetGrain<IPollingConnectionWorker>(internalKey);
                await pollingConnection.DisconnectAsync();

                State.Logins.Remove(connectionKey);
                await WriteStateAsync();
            }
        }

        public Task<IEnumerable<ConnectionKey>> GetLoginsAsync()
        {
            return Task.FromResult(State.Logins.Keys.AsEnumerable());
        }
    }
}