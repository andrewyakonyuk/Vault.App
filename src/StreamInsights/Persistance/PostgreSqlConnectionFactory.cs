using Npgsql;
using System;
using System.Data;

namespace StreamInsights.Persistance
{
    public class PostgreSqlConnectionFactory : ISqlConnectionFactory
    {
        readonly string _connectionString;

        public PostgreSqlConnectionFactory(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
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
