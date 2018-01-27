using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.WebApp.Infrastructure.Persistence
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}
