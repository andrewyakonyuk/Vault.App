using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Vault.Shared.Identity.Overrides
{
    public class IdentityUserOverride : IAutoMappingOverride<IdentityUser>
    {
        public void Override(AutoMapping<IdentityUser> mapping)
        {
            mapping.HasMany(t => t.Logins).KeyColumn("user_id");
            mapping.HasMany(t => t.Claims).KeyColumn("user_id");
        }
    }
}