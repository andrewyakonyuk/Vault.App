using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace Vault.Shared.Identity.Overrides
{
    public class IdentityUserLoginOverride : IAutoMappingOverride<IdentityUserLogin>
    {
        public void Override(AutoMapping<IdentityUserLogin> mapping)
        {
            mapping.HasOne(t => t.User).ForeignKey("UserId").Cascade.SaveUpdate();
        }
    }
}
