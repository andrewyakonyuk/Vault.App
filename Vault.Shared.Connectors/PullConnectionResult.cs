using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Connectors
{
    public class PullConnectionResult
    {
        public readonly static PullConnectionResult Empty = new PullConnectionResult { IsCancellationRequested = true };

        public bool IsCancellationRequested { get; set; }
    }
}
