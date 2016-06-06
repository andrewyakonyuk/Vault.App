using System;
using System.Threading;

namespace Vault.Shared.Connectors
{
    public class PullConnectionContext
    {
        public PullConnectionContext(UserInfo user, CancellationToken token)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            User = user;
            CancellationToken = token;
        }

        public PullConnectionContext(PullConnectionState state, CancellationToken token)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            if (string.IsNullOrEmpty(state.ProviderKey))
                throw new ArgumentNullException(nameof(state.ProviderKey));

            User = new UserInfo(state.ProviderKey);
            Iteration = state.Iteration;
            LastRunTime = state.LastRunTime;
            RecoveryMode = !state.IsLastSucceded;
        }

        public int Iteration { get; set; }

        public UserInfo User { get; private set; }

        public DateTime? LastRunTime { get; set; }

        public bool RecoveryMode { get; set; }

        public CancellationToken CancellationToken { get; private set; }
    }
}
