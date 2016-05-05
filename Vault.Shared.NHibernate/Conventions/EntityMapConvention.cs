using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Vault.Shared.NHibernate.Conventions
{
    public class EntityMapConvention : IClassConvention, IJoinedSubclassConvention
    {
        public void Apply(IClassInstance instance)
        {
            string tableName = NameConventions.GetTableName(instance.EntityType);

            instance.Table(tableName);
            instance.BatchSize(25);
        }

        public void Apply(IJoinedSubclassInstance instance)
        {
            string tableName = NameConventions.GetTableName(instance.EntityType);

            instance.Table(tableName);
            instance.BatchSize(25);
        }
    }
}