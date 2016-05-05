using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Framework.Models.Overrides
{
    public class MusicGroupMapping : IAutoMappingOverride<MusicGroup>
    {
        public void Override(AutoMapping<MusicGroup> mapping)
        {
            mapping.HasMany(t => t.Albums).Cascade.SaveUpdate().AsList();
        }
    }
}