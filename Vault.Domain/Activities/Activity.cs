using System;
using Vault.Domain.Models;
using Vault.Shared.Domain;
using Vault.Shared.Events;

namespace Vault.Domain.Activities
{
    public abstract class Activity : IEvent
    {
        public virtual int OwnerId { get; set; }

        /// <summary>
        /// The object upon the action is carried out, whose state is kept intact or changed.
        /// Also known as the semantic roles patient, affected or undergoer (which change their state)
        /// or theme (which doesn't)
        /// </summary>
        //public virtual int ThingId { get; set; }

        public virtual Thing Affected { get; set; }

        public virtual DateTime? StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Indicates the current disposition of the Action.
        /// </summary>
        public virtual ActionStatusType Status { get; set; }

        /// <summary>
        /// Indicates a target EntryPoint for an Action.
        /// </summary>
        public virtual EntryPoint Target { get; set; }
    }

    /// <summary>
    /// The status of an Action.
    /// </summary>
    public enum ActionStatusType
    {
        /// <summary>
        /// An action that has already taken place.
        /// </summary>
        Completed = 0,

        /// <summary>
        /// A description of an action that is supported.
        /// </summary>
        Potential,

        /// <summary>
        /// An in-progress action (e.g, while watching the movie, or driving to a location).
        /// </summary>
        Active,

        /// <summary>
        /// An action that failed to complete.
        /// </summary>
        Failed
    }
}