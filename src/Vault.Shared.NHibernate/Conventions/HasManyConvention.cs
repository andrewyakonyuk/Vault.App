using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Vault.Shared.NHibernate.Conventions
{
    public class HasManyConvention : IHasManyConvention, IHasManyToManyConvention
    {
        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Access.ReadOnlyPropertyThroughCamelCaseField(CamelCasePrefix.Underscore);
            instance.Cascade.AllDeleteOrphan();
            instance.AsSet();
            instance.BatchSize(25);
            if (instance.OtherSide == null)
            {
                instance.Not.Inverse();
            }
            else
            {
                instance.Inverse();
            }
            instance.Not.KeyNullable();
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Access.ReadOnlyPropertyThroughCamelCaseField(CamelCasePrefix.Underscore);
            instance.Cascade.SaveUpdate();
            instance.AsSet();
            instance.BatchSize(25);
        }
    }
}