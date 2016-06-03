using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Routing;
using System;
using System.Threading.Tasks;
using Vault.Shared.Identity;

namespace Vault.Framework.Mvc.Routing
{
    public class WorkContextAwareRoute : IRouter
    {
        private readonly IRouter _next;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public WorkContextAwareRoute(
            IRouter next,
            IWorkContextAccessor workContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            if (workContextAccessor == null)
                throw new ArgumentNullException(nameof(workContextAccessor));
            if (userManager == null)
                throw new ArgumentNullException(nameof(userManager));

            _next = next;
            _workContextAccessor = workContextAccessor;
            _userManager = userManager;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return _next.GetVirtualPath(context);
        }

        public async Task RouteAsync(RouteContext context)
        {
            context.RouteData.Routers.Add(_next);

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                _workContextAccessor.WorkContext.User = await _userManager.FindByNameAsync(context.HttpContext.User.Identity.Name);

                if (!context.RouteData.Values.ContainsKey("username"))
                {
                    context.RouteData.Values.Add("username", context.HttpContext.User.Identity.Name);
                }
            }

            if (context.RouteData.Values.ContainsKey("username"))
            {
                var username = (string)context.RouteData.Values["username"];
                _workContextAccessor.WorkContext.Owner = await _userManager.FindByNameAsync(username);
            }

            await _next.RouteAsync(context);
        }
    }
}