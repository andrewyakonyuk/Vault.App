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

        public int Iteration { get; set; }

        public UserInfo User { get; private set; }

        public DateTime? LastRunTime { get; set; }

        public CancellationToken CancellationToken { get; private set; }
    }
}
