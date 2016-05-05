namespace Vault.Shared.Commands
{
    public interface ICommandBuilder
    {
        CommandResult Execute<TCommandContext>(TCommandContext commandContext)
            where TCommandContext : ICommandContext;
    }
}