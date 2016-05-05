using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Vault.Framework.Models.Activities;

namespace Vault.Framework.Models.Overrides
{
    public class ActivityMapping : IAutoMappingOverride<Activity>
    {
        public void Override(AutoMapping<Activity> mapping)
        {
            mapping.DiscriminateSubClassesOnColumn("ActivityType");
        }
    }
}