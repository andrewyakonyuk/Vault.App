using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Vault.WebHost.Infrastructure.Mvc.Routing;

namespace Microsoft.AspNetCore.Builder
{
    public static class RouteBuilderExtensions
    {
        public static IRouteBuilder MapRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string template,
            object defaults = null,
            object constraints = null,
            object projections = null,
            object dataTokens = null)
        {
            if (routeBuilder.DefaultHandler == null)
            {
                throw new InvalidOperationException("DefaultHandler_MustBeSet");
            }

            IInlineConstraintResolver requiredService = routeBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();
            routeBuilder.Routes.Add(new ProjectionRoute(routeBuilder.DefaultHandler, name, template, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new RouteValueDictionary(projections), new RouteValueDictionary(dataTokens), requiredService));
            return routeBuilder;
        }
    }
}