using FluentNHibernate.Automapping;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared.Domain;

namespace Vault.Shared.NHibernate
{
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool AbstractClassIsLayerSupertype(Type type)
        {
            return true;
        }

        public override bool IsComponent(Type type)
        {
            return typeof(IEntityComponent).IsAssignableFrom(type);
        }

        public override bool IsDiscriminated(Type type)
        {
            return true;
        }

        public override bool ShouldMap(Type type)
        {
            return typeof(IEntity).IsAssignableFrom(type)
                && !typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type) //hack: special case. thinking about ignore attribute
                ;
        }
    }
}