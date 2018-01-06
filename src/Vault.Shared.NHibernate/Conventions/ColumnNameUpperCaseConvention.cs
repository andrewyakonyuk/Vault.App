using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Vault.Shared.NHibernate.Conventions
{
    public class ColumnNameUpperCaseConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            var name = NameConventions.ReplaceCamelCaseWithUnderscore(instance.Name);

            instance.Column(name.ToUpper());
        }
    }
}