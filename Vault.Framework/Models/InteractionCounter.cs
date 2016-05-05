using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Framework.Models.Activities;
using Vault.Shared.Domain;

namespace Vault.Framework.Models
{
    public class InteractionCounter : IEntity, IContent
    {
        public virtual int Id { get; set; }

        public virtual int OwnerId { get; set; }

        public virtual DateTime Published { get; set; }

        public virtual Activity InteractionType { get; set; }

        public virtual int InteractionCount { get; set; }

        public virtual InteractionService InteractionService { get; set; }
    }

    public class InteractionService : Thing
    {
        public virtual string Type { get; set; }
    }
}