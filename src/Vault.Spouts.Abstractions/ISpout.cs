using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Spouts.Abstractions
{
    public interface ISpout<T>
    {
        Task<ConsumeResult<T>> ConsumeAsync(ConsumeMessageContext context);
    }
}
