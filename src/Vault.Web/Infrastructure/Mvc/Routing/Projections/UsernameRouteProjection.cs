using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Vault.WebHost.Infrastructure.Mvc.Routing.Projections
{
    public class UsernameRouteProjection : IRouteProjection
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public UsernameRouteProjection(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public void Incoming(string key, IDictionary<string, object> values)
        {
        }

        public void Outgoing(string key, IDictionary<string, object> values)
        {
            var currentUser = _workContextAccessor.WorkContext.User;

            if (currentUser == null)
                return;

            if (string.Equals(key, "username", StringComparison.InvariantCultureIgnoreCase))
            {
                values.TryGetValue(key, out object tempUsername);
                string username = tempUsername as string;
                if (string.IsNullOrEmpty(username))
                {
                    values[key] = currentUser.UserName;
                }
            }
        }
    }
}

