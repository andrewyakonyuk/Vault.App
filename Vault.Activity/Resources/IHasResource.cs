namespace Vault.Activity.Resources
{
    public interface IHasResource<TResource>
    {
        TResource Resource { get; }
    }
}