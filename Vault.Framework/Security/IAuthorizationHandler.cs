namespace Vault.Framework.Security
{
    public interface IAuthorizationHandler
    {
        void Checking(CheckAccessContext context);

        void Adjust(CheckAccessContext context);

        void Complete(CheckAccessContext context);
    }
}