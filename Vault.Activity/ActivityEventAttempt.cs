using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Activity
{
    [Serializable]
    [Immutable]
    public partial class ActivityEventAttempt : ActivityEntry
    {
    }
}
