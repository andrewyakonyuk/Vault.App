using System;

namespace Vault.Shared.Commands
{
    public sealed class CommandResult : IEquatable<CommandResult>
    {
        private enum CommandStatus
        {
            Unknown = 0,
            Accept = 1,
            Decline = 2
        }

        private CommandStatus _status;

        public string Message { get; private set; }

        public static CommandResult Decline(string reason)
        {
            return new CommandResult
            {
                _status = CommandStatus.Decline,
                Message = reason
            };
        }

        public static CommandResult Decline()
        {
            return Decline(null);
        }

        public static CommandResult Accept()
        {
            return new CommandResult
            {
                _status = CommandStatus.Accept
            };
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CommandResult);
        }

        public bool Equals(CommandResult result)
        {
            if (result == null)
                return false;

            return _status == result._status;
        }

        public override int GetHashCode()
        {
            if (_status == CommandStatus.Unknown)
                return 0;

            return _status.GetHashCode();
        }

        public static bool operator ==(CommandResult left, CommandResult right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(CommandResult left, CommandResult right)
        {
            return !(left == right);
        }
    }
}