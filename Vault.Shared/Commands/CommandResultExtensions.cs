namespace Vault.Shared.Commands
{
    public static class CommandResultExtensions
    {
        public static bool Accepted(this CommandResult result)
        {
            if (result == null)
                return false;

            return result == CommandResult.Accept();
        }
    }
}
