using System;
using System.Collections.Generic;

namespace Vault.Spouts.Abstractions
{
    public class ConsumeMessageContext
    {
        public ConsumeMessageContext(UserInfo user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Properties = new Dictionary<string, object>();
        }
        
        public int Batch { get; set; }
        
        public UserInfo User { get; }
        
        public DateTimeOffset? LastFetchTimeUtc { get; set; }

        public IDictionary<string, object> Properties { get; }
    }
}
