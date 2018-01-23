using System.Collections.Generic;

namespace Vault.WebApp.Infrastructure.Mvc.Routing
{
    public interface IRouteProjection
    {
        void Incoming(string key, IDictionary<string, object> values);

        void Outgoing(string key, IDictionary<string, object> values);
    }
}