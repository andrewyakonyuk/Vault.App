using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Vault.Shared.Identity.Overrides
{
    public class IdentityUserClaimOverride : IAutoMappingOverride<IdentityUserClaim>
    {
        public void Override(AutoMapping<IdentityUserClaim> mapping)
        {
            mapping.References(t => t.User, "user_id").Fetch.Join();
        }
    }
}