using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Vault.Shared.Identity.Overrides
{
    public class IdentityUserClaimOverride : IAutoMappingOverride<IdentityUserClaim>
    {
        public void Override(AutoMapping<IdentityUserClaim> mapping)
        {
            mapping.HasOne(t => t.User).ForeignKey("UserId").Cascade.SaveUpdate();
        }
    }
}
