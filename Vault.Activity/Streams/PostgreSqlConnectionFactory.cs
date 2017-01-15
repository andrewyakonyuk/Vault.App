using System;
using System.Data;
using NEventStore.Persistence.Sql;
using Npgsql;

namespace Vault.Activity.Streams
{
    public class PostgreSqlConnectionFactory : IConnectionFactory
    {
        readonly string _connectionString;

        public PostgreSqlConnectionFactory(string connectionString)
        {
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