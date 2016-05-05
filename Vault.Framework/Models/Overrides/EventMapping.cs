using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Vault.Framework.Models.Overrides
{
    public class EventMapping : IAutoMappingOverride<Event>
    {
        public void Override(AutoMapping<Event> mapping)
        {
            mapping.References(t => t.Location).Cascade.SaveUpdate();
        }
    }
}