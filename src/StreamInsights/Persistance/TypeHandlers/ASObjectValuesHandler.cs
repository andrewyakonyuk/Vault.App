using Newtonsoft.Json;
using Npgsql;
using StreamInsights.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static Dapper.SqlMapper;

namespace StreamInsights.Persistance.TypeHandlers
{
    public class ASObjectValuesHandler : TypeHandler<Values<ASObject>>
    {
        public static ASObjectValuesHandler Default { get; } = new ASObjectValuesHandler();
        
        readonly JsonSerializerSettings _jsonSerializerSettings;

        ASObjectValuesHandler()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                Converters = new[] { new ValuesConverter() }
            };
        }

        public override Values<ASObject> Parse(object value)
        {
            var json = value as string;
            if (string.IsNullOrEmpty(json))
                return Values<ASObject>.Empty;

            var result = JsonConvert.DeserializeObject<List<ASObject>>(json, _jsonSerializerSettings);
            if (result == null || result.Count == 0)
                return Values<ASObject>.Empty;

            if (result.Count == 1)
                return result[0];

            return result;
        }

        public override void SetValue(IDbDataParameter parameter, Values<ASObject> value)
        {
            if (parameter is NpgsqlParameter npgsqlParameter)
            {
                npgsqlParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            }

            if (value.HasValue)
            {
                var list = (List<ASObject>)value;
                parameter.Value = JsonConvert.SerializeObject(list, Formatting.None, _jsonSerializerSettings);
            }
            else
            {
                parameter.Value = DBNull.Value;
            }
        }
    }
}
