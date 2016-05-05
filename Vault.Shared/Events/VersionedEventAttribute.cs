using System;

namespace Vault.Shared.Events
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VersionedEventAttribute : Attribute
    {
        public int Version { get; set; }
        public string Identifier { get; set; }

        public VersionedEventAttribute(string identifier, int version = 0)
        {
            this.Version = version;
            this.Identifier = identifier;
        }
    }
}