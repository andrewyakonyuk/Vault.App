using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.NHibernate
{
    ///<summary>
    ///  Bootstrapper for nhibernate
    ///</summary>
    public interface INHibernateInitializer
    {
        ///<summary>
        ///  Builds and returns nhibernate configuration
        ///</summary>
        ///<returns> NHibernate configuration object </returns>
        Configuration GetConfiguration();
    }
}
