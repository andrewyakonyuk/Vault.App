using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Activity.Services
{
    public interface IClock
    {
        DateTimeOffset Now { get; }
    }

    public class DefaultClock : IClock
    {
        public DateTimeOffset Now
        {
            get
            {
                return DateTimeOffset.Now;
            }
        }
    }
}