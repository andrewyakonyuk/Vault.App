using System;
using System.Collections.Generic;
using Vault.Shared.Domain;

namespace Vault.Framework.Identity
{
    /// <summary>
    /// Represents a user in the identity system
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for the user.</typeparam>
    public class IdentityUser : IEntity, IUser
    {
        private List<IdentityUserClaim> _claims;
        private List<IdentityUserLogin> _logins;

        /// <summary>
        /// Initializes a new instance of <see cref="T:Vault.App.Authentication.IdentityUser`1" />.
        /// </summary>
        public IdentityUser()
        {
            this.ConcurrencyStamp = Guid.NewGuid().ToString();
            _claims = new List<IdentityUserClaim>();
            _logins = new List<IdentityUserLogin>();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="T:Vault.App.Authentication.IdentityUser`1" />.
        /// </summary>
        /// <param name="userName">The user name.</param>
        public IdentityUser(string userName) : this()
        {
            this.UserName = userName;
        }

        /// <summary>
        /// Returns the username for this user.
        /// </summary>
        public override string ToString()
        {
            return this.UserName;
        }

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        public virtual int AccessFailedCount
        {
            get; set;
        }

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim> Claims
        {
            get
            {
                return _claims;
            }
        }

        /// <summary>
        /// A random value that must change whenever a user is persisted to the store
        /// </summary>
        public virtual string ConcurrencyStamp
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the email address for this user.
        /// </summary>
        public virtual string Email
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their email address.
        /// </summary>
        /// <value>True if the email address has been confirmed, otherwise false.</value>
        public virtual bool EmailConfirmed
        {
            get; set;
        }

        /// <summary>
        /// </summary>
        /// Gets or sets the primary key for this user.
        public virtual int Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a flag indicating if this user is locked out.
        /// </summary>
        /// <value>True if the user is locked out, otherwise false.</value>
        public virtual bool LockoutEnabled
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when any user lockout ends.
        /// </summary>
        /// <remarks>
        /// A value in the past means the user is not locked out.
        /// </remarks>
        public virtual DateTime? LockoutEnd
        {
            get; set;
        }

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<IdentityUserLogin> Logins
        {
            get
            { return _logins; }
        }

        /// <summary>
        /// Gets or sets the normalized email address for this user.
        /// </summary>
        public virtual string NormalizedEmail
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the normalized user name for this user.
        /// </summary>
        public virtual string NormalizedUserName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        public virtual string PasswordHash
        {
            get; set;
        }

        /// <summary>
        /// A random value that must change whenever a users credentials change (password changed, login removed)
        /// </summary>
        public virtual string SecurityStamp
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        public virtual string UserName { get; set; }
    }
}