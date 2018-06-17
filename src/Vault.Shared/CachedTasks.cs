using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Shared
{
    public static class CachedTasks
    {
        public static readonly Task<bool> BooleanTrue = Task.FromResult(true);
        public static readonly Task<bool> BooleanFalse = Task.FromResult(false);
    }
}
