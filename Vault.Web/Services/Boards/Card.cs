using System;
using System.Collections.Generic;
using Vault.Shared;

namespace Vault.WebHost.Services.Boards
{
    public abstract class Card : IContent
    {
        public int Id { get; set; }

        public DateTime Published { get; set; }

        public SourceInfo Source { get; set; }

        public int OwnerId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string[] Tags { get; set; }
    }
}