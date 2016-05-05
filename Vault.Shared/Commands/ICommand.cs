namespace Vault.Shared.Commands
{
    public interface ICommand<in TCommandContext>
         where TCommandContext : ICommandContext
    {
        void Execute(TCommandContext context);
    }
}