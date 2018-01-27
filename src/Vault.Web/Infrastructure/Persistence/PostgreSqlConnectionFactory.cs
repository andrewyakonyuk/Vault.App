using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.WebApp.Infrastructure.Persistence
{
    public class PostgreSqlConnectionFactory : IDbConnectionFactory
    {
        protected readonly SqlConnectionFactoryOptions _options;

        public PostgreSqlConnectionFactory(IOptions<SqlConnectionFactoryOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.Value.ConnectionString))
                throw new ArgumentNullException(nameof(SqlConnectionFactoryOptions.ConnectionString));

            _options = options.Value;
        }

        public Type GetDbProviderFactoryType()
        {
            return typeof(NpgsqlFactory);
        }

        public IDbConnection Create()
        {
            var connection = NpgsqlFactory.Instance.CreateConnection();
            connection.ConnectionString = _options.ConnectionString;
            connection.Open();
            return connection;
        }
    }
}
