using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Vault.WebApp.Infrastructure.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Vault.WebApp.Infrastructure.Mvc
{
    public class WorkContextAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public WorkContextAuthorizationFilter(
            IWorkContextAccessor workContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _workContextAccessor = workContextAccessor ?? throw new ArgumentNullException(nameof(workContextAccessor));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
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
        }
    }
}