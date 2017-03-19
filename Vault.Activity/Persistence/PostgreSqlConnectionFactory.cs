using System;
using System.Data;
using System.Data.Common;
using NEventStore.Persistence.Sql;
using Npgsql;

namespace Vault.Activity.Persistence
{
    public class PostgreSqlConnectionFactory : IConnectionFactory, ISqlConnectionFactory
    {
        readonly string _connectionString;

        public PostgreSqlConnectionFactory(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
        }

        public Type GetDbProviderFactoryType()
        {
            return typeof(NpgsqlFactory);
        }

        public IDbConnection Open()
        {
            return Open(_connectionString);
        }

        protected virtual IDbConnection Open(string connectionString)
        {
            var connection = NpgsqlFactory.Instance.CreateConnection();
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }
    }
}