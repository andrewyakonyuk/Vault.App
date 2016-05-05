using System;

namespace Vault.Shared.Commands
{
    public class CommandBuilder : ICommandBuilder
    {
        private readonly ICommandFactory _commandFactory;
        private readonly ILogger _logger;

        public CommandBuilder(ICommandFactory commandFactory, ILogger logger)
        {
            _commandFactory = commandFactory;
            _logger = logger;
        }

        public CommandResult Execute<TCommandContext>(TCommandContext commandContext)
            where TCommandContext : ICommandContext
        {
            var command = _commandFactory.Create<TCommandContext>();
            try
            {
                command.Execute(commandContext);
                return CommandResult.Accept();
            }
            catch (ArgumentException ex)
            {
                _logger.WriteError("Exception occurs while executing command '{0}'. See details: '{1}'", command.GetType().FullName, ex.Message);
                return CommandResult.Decline(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.WriteError("Exception occurs while executing command '{0}'. See details: '{1}'", command.GetType().FullName, ex.Message);
                return CommandResult.Decline(ex.Message);
            }
            catch (VaultException ex)
            {
                _logger.WriteError("Exception occurs while executing command '{0}'. See details: '{1}'", command.GetType().FullName, ex.Message);
                return CommandResult.Decline(ex.Message);
            }
        }
    }
}