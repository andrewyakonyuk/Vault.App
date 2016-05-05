using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Vault.Shared.NHibernate.Conventions
{
    public class PrimaryKeyConvention : IIdConvention
    {
        public void Apply(IIdentityInstance instance)
        {
            string sequenceName = NameConventions.GetSequenceName(instance.EntityType);
            string columnName = NameConventions.GetPrimaryKeyColumnName(instance.EntityType);

            instance.Column(columnName);
            instance.GeneratedBy.Native(paramBuilder => paramBuilder.AddParam("sequence", sequenceName));
        }
    }
}