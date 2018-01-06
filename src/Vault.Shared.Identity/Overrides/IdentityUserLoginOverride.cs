using System;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Vault.Shared.Identity.Overrides
{
    public class IdentityUserLoginOverride : IAutoMappingOverride<IdentityUserLogin>
    {
        public void Override(AutoMapping<IdentityUserLogin> mapping)
        {
            mapping.References(t => t.User, "user_id").Fetch.Join();
        }
    }
}