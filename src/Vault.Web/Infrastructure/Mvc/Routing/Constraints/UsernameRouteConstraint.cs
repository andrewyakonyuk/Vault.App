using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Vault.Shared.Identity;
using Vault.Shared.Identity.Query;
using Vault.Shared.Queries;

namespace Vault.WebApp.Infrastructure.Mvc.Routing.Constraints
{
    public class UsernameRouteConstraint : IRouteConstraint
    {
        readonly IQueryBuilder _query;

        public UsernameRouteConstraint(IQueryBuilder query)
        {
            _query = query;
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (routeDirection == RouteDirection.IncomingRequest)
            {
                object tempValue;
                if (values.TryGetValue(routeKey, out tempValue) && tempValue != null)
                {
                    var user = _query.For<IdentityUser>().With(new Username((string)tempValue)).Result;
                    return user != null;
                }
            }
            return true;
        }
    }
}