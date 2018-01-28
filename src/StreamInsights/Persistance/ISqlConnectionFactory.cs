using System.Data;

namespace StreamInsights.Persistance
{
    public interface ISqlConnectionFactory
    {
        IDbConnection Open();
    }
}
