using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared;
using Vault.Shared.Queries;

namespace Vault.Framework.Identity.Query
{
    public class Username : ICriterion, ICacheKeyProvider
    {
        readonly string _username;

        public Username(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

            _username = username;
        }

        public string Value { get { return _username; } }

        string ICacheKeyProvider.CacheKey
        {
            get
            {
                return string.Join(":", "FindByUsername:{0}", _username.ToLowerInvariant());
            }
        }
    }
}