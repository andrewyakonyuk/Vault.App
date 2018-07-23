using Npgsql;
using StreamInsights.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static Dapper.SqlMapper;

namespace StreamInsights.Persistance.TypeHandlers
{
    public class StringValuesHandler : TypeHandler<Values<string>>
    {
        public static StringValuesHandler Default { get; } = new StringValuesHandler();

        public override Values<string> Parse(object value)
        {
            var array = value as string[];
            if (array == null)
                return Values<string>.Empty;

            if (array.Length == 0)
                return Values<string>.Empty;

            return array;
        }

        public override void SetValue(IDbDataParameter parameter, Values<string> value)
        {
            if (parameter is NpgsqlParameter npgsqlParameter)
            {
                npgsqlParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text | NpgsqlTypes.NpgsqlDbType.Array;
            }

            if (Values.IsNullOrEmpty(value))
                parameter.Value = Array.Empty<string>();
            else
                parameter.Value = (List<string>)value;
        }
    }
}
