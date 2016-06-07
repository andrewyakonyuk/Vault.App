using System;
using System.Collections.Generic;
using System.Linq;

namespace Vault.Shared.Commands
{
    public class CommandBuilder : ICommandBuilder
    {
        private readonly ICommandFactory _commandFactory;
        private readonly ILogger _logger;
        private IList<ICommandInterceptor> _interceptors;

        public CommandBuilder(
            ICommandFactory commandFactory,
            IEnumerable<ICommandInterceptor> interceptors,
            ILogger logger)
        {
            _commandFactory = commandFactory;
            _interceptors = interceptors.OrderByDescending(t => t.Order).ToArray();
            _logger = logger;
        }

        public CommandResult Execute<TCommandContext>(TCommandContext commandContext)
            where TCommandContext : ICommandContext
        {
            var command = _commandFactory.Create<TCommandContext>();
            var executeMethod = BuildMethod(command);
            try
            {
                return executeMethod(commandContext);
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

        private ExecuteCommandMethod<TCommandContext> BuildMethod<TCommandContext>(ICommand<TCommandContext> command)
            where TCommandContext : ICommandContext
        {
            ExecuteCommandMethod<TCommandContext> result = (context) =>
            {
                command.Execute(context);
                return CommandResult.Accept();
            };

            for (int i = _interceptors.Count - 1; i >= 0; i--)
            {
                var item = _interceptors[i];

                var inner = result;
                result = (context) => item.Execute(context, inner);
            }
            return result;
        }
    }
}