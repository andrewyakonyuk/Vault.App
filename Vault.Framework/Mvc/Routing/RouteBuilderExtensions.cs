using Microsoft.AspNet.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Vault.Framework.Mvc.Routing;

namespace Microsoft.AspNet.Builder
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

            IInlineConstraintResolver constraintResolver = routeBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();

            var templateRoute = new ProjectionTemplateRoute(routeBuilder.DefaultHandler, name, template,
                ObjectToDictionary(defaults), ObjectToDictionary(constraints),
                ObjectToDictionary(projections), ObjectToDictionary(dataTokens), constraintResolver);

            routeBuilder.Routes.Add(templateRoute);
            return routeBuilder;
        }

        private static IDictionary<string, object> ObjectToDictionary(object value)
        {
            IDictionary<string, object> dictionary = value as IDictionary<string, object>;
            if (dictionary != null)
            {
                return dictionary;
            }
            return new RouteValueDictionary(value);
        }
    }
}