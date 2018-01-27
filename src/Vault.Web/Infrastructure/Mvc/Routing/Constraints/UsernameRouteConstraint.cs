using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Vault.WebApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Vault.WebApp.Infrastructure.Mvc.Routing.Constraints
{
    public class UsernameRouteConstraint : IRouteConstraint
    {
        readonly UserManager<IdentityUser> _userManager;

        public UsernameRouteConstraint(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (routeDirection == RouteDirection.IncomingRequest)
            {
                if (values.TryGetValue(routeKey, out object tempValue) && tempValue != null)
                {
                    var user = _userManager.FindByNameAsync((string)tempValue).GetAwaiter().GetResult();
                    return user != null;
                }
            }
            return true;
        }
    }
}