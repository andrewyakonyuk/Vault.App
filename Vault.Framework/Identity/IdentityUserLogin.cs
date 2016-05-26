using Vault.Shared.Domain;

namespace Vault.Framework.Identity
{
    /// <summary>
    /// Represents a login and its associated provider for a user.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key of the user associated with this login.</typeparam>
    public class IdentityUserLogin : IEntity
    {
        /// <summary>
        /// Gets or sets the login provider for the login (e.g. facebook, google)
        /// </summary>
        public virtual string LoginProvider
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the friendly name used in a UI for this login.
        /// </summary>
        public virtual string ProviderDisplayName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the unique provider identifier for this login.
        /// </summary>
        public virtual string ProviderKey
        {
            get; set;
        }

        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets the user associated with this login.
        /// </summary>
        public virtual IdentityUser User { get; set; }
    }
}