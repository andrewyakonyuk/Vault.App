using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.Data;
using static Dapper.SqlMapper;

namespace StreamInsights.Persistance.TypeHandlers
{
    public class JObjectHandler : TypeHandler<JObject>
    {
        private JObjectHandler() { }

        public static JObjectHandler Default { get; } = new JObjectHandler();

        public override JObject Parse(object value)
        {
            var json = (string)value;
            return json == null ? null : JObject.Parse(json);
        }

        public override void SetValue(IDbDataParameter parameter, JObject value)
        {
            if (parameter is NpgsqlParameter npgsqlParameter)
            {
                npgsqlParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            }

            parameter.Value = value?.ToString(Formatting.None);
        }
    }
}
