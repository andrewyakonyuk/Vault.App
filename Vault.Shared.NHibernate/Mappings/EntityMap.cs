using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared.Domain;

namespace Vault.Shared.NHibernate.Mappings
{
    public abstract class EntityMap<TEntity> : ClassMap<TEntity>
        where TEntity : class, IEntity
    {
        protected EntityMap()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id(x => x.Id);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

            DynamicInsert();
            DynamicUpdate();
        }
    }
}
