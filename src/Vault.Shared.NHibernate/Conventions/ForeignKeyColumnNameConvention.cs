using FluentNHibernate;
using FluentNHibernate.Conventions;
using System;

namespace Vault.Shared.NHibernate.Conventions
{
    public class ForeignKeyColumnNameConvention : ForeignKeyConvention
    {
        protected override string GetKeyName(Member member, Type type)
        {
            string name = NameConventions.ReplaceCamelCaseWithUnderscore(member == null ? type.Name : member.Name);

            return string.Format("{0}_ID", name.ToUpper());
        }
    }
}