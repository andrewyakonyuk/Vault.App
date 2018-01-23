using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Vault.WebApp.Services.Boards.Overrides
{
    public class BoardMapping : IAutoMappingOverride<Board>
    {
        public void Override(AutoMapping<Board> mapping)
        {
            mapping.IgnoreProperty(t => t.Cards);
        }
    }
}