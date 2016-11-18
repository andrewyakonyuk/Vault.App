using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vault.Shared;
using Vault.Shared.Domain;

namespace Vault.WebHost.Services.Boards
{
    [DataContract]
    public class Board : IContent, IEntity, ICloneable
    {
        public Board()
        {
            Cards = PagedEnumerable.Empty<Card>();
            RawQuery = string.Empty;
        }

        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual int Id { get; set; }

        public virtual IPagedEnumerable<Card> Cards { get; set; }

        [DataMember]
        public virtual int OwnerId { get; set; }

        [DataMember]
        public virtual string RawQuery { get; set; }

        [DataMember]
        public virtual DateTime Published { get; set; }

        public virtual object Clone()
        {
            return new Board
            {
                Id = Id,
                RawQuery = RawQuery,
                Name = Name,
                OwnerId = OwnerId,
                Published = Published
            };
        }
    }
}