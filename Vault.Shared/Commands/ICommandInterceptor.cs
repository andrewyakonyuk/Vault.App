using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Commands
{
    public interface ICommandInterceptor
    {
        int Order { get; }

        CommandResult Execute<TCommandContext>(TCommandContext commandContext, ExecuteCommandMethod<TCommandContext> execute)
            where TCommandContext : ICommandContext;
    }

    public delegate CommandResult ExecuteCommandMethod<in TCommandContext>(TCommandContext commandContext)
        where TCommandContext : ICommandContext;
}
