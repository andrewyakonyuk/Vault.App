using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.NHibernate.Conventions
{
    public class UriConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            if (typeof(Uri).IsAssignableFrom(instance.Property.PropertyType))
                instance.CustomType<UriType>();
        }
    }
}