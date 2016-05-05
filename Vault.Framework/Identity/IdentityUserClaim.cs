﻿using Vault.Shared.Domain;

namespace Vault.Framework.Identity
{
    /// <summary>
    /// Represents a claim that a user possesses.
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for this user that possesses this claim.</typeparam>
    public class IdentityUserClaim : IEntity
    {
        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        public virtual string ClaimType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        public virtual string ClaimValue
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the identifier for this user claim.
        /// </summary>
        public virtual int Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the of the primary key of the user associated with this claim.
        /// </summary>
        public virtual int UserId { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}