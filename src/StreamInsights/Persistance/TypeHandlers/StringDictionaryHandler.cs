using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using static Dapper.SqlMapper;

namespace StreamInsights.Persistance.TypeHandlers
{
    public class StringDictionaryHandler : TypeHandler<Dictionary<string, string>>
    {
        public static StringDictionaryHandler Default { get; } = new StringDictionaryHandler();

        public override Dictionary<string, string> Parse(object value)
        {
            var json = (string)value;
            if (string.IsNullOrEmpty(json))
                return null;

            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return result;
        }

        public override void SetValue(IDbDataParameter parameter, Dictionary<string, string> value)
        {
            if (parameter is NpgsqlParameter npgsqlParameter)
            {
                npgsqlParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            }

            if (value == null)
                parameter.Value = DBNull.Value;
            else
            {
                var json = JsonConvert.SerializeObject(value, Formatting.None);
                parameter.Value = json;
            }
        }
    }
}
