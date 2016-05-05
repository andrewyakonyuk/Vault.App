using Microsoft.AspNet.Mvc.Filters;
using System;
using Vault.Framework.Identity;
using Vault.Framework.Identity.Query;
using Vault.Shared.Queries;

namespace Vault.Framework.Mvc
{
    public class WorkContextAwareFilter : IActionFilter
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IQueryBuilder _queryBuilder;

        public WorkContextAwareFilter(
            IWorkContextAccessor workContextAccessor,
            IQueryBuilder queryBuilder)
        {
            if (workContextAccessor == null)
                throw new ArgumentNullException(nameof(workContextAccessor));
            if (queryBuilder == null)
                throw new ArgumentNullException(nameof(queryBuilder));

            _workContextAccessor = workContextAccessor;
            _queryBuilder = queryBuilder;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public async void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                _workContextAccessor.WorkContext.User = await _queryBuilder.For<IdentityUser>().With(new Username(context.HttpContext.User.Identity.Name));

                if (!context.RouteData.Values.ContainsKey("username"))
                {
                    context.RouteData.Values.Add("username", context.HttpContext.User.Identity.Name);
                }
            }

            if (context.RouteData.Values.ContainsKey("username"))
            {
                var username = (string)context.RouteData.Values["username"];
                _workContextAccessor.WorkContext.Owner = await _queryBuilder.For<IdentityUser>().With(new Username(context.HttpContext.User.Identity.Name));
            }
        }
    }
}