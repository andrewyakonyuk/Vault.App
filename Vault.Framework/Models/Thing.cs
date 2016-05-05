using System;
using System.Collections.Generic;
using Vault.Framework.Models.Activities;
using Vault.Shared.Domain;

namespace Vault.Framework.Models
{
    public abstract class Thing : IEntity
    {
        ICollection<InteractionCounter> _interactions = new List<InteractionCounter>();

        public virtual int Id { get; set; }

        /// <summary>
        /// An alias for the item.
        /// </summary>
        public virtual string AlternativeName { get; set; }

        /// <summary>
        /// A short description of the item.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// The name of the item.
        /// </summary>
        public virtual string Name { get; set; }

        //todo: map abstract class via fluent nhibernate
        ///public Thing SameAs { get; set; }

        /// <summary>
        /// URL of the item.
        /// </summary>
        public virtual string Url { get; set; }

        /// <summary>
        /// An additional type for the item, typically used for adding more specific types.
        /// This is a relationship between something and a class that the thing is in.
        /// </summary>
        public virtual string AdditionalType { get; set; }

        public virtual Image Image { get; set; }

        public virtual ICollection<InteractionCounter> Interactions { get { return _interactions; } }
    }
}