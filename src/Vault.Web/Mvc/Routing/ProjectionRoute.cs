using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace Vault.WebHost.Mvc.Routing
{
    public class ProjectionRoute : Route
    {
        public virtual RouteValueDictionary Projections { get; protected set; }

        public ProjectionRoute(
            IRouter target,
            string routeTemplate,
            IInlineConstraintResolver inlineConstraintResolver)
            : this(target, routeTemplate, null, null, null, null, inlineConstraintResolver)
        {
        }

        public ProjectionRoute(
            IRouter target,
            string routeTemplate,
            RouteValueDictionary defaults,
            RouteValueDictionary constraints,
            RouteValueDictionary projections,
            RouteValueDictionary dataTokens,
            IInlineConstraintResolver inlineConstraintResolver)
            : this(target, null, routeTemplate, defaults, constraints, projections, dataTokens, inlineConstraintResolver)
        {
        }

        public ProjectionRoute(
            IRouter target,
            string routeName,
            string routeTemplate,
           RouteValueDictionary defaults,
            RouteValueDictionary constraints,
            RouteValueDictionary projections,
            RouteValueDictionary dataTokens,
            IInlineConstraintResolver inlineConstraintResolver)
                : base(target,
                routeName,
                routeTemplate,
                defaults,
                constraints,
                dataTokens,
                inlineConstraintResolver)
        {
            Projections = projections;
        }

        public override VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            foreach (var item in Projections)
            {
                if (item.Value == null)
                    continue;

                ((IRouteProjection)item.Value).Outgoing(item.Key, context.Values);
            }

            return base.GetVirtualPath(context);
        }

        public override Task RouteAsync(RouteContext context)
        {
            foreach (var item in Projections)
            {
                if (item.Value == null)
                    continue;

                ((IRouteProjection)item.Value).Incoming(item.Key, context.RouteData.Values);
            }

            return base.RouteAsync(context);
        }

        private static IReadOnlyDictionary<string, IRouteProjection> GetProjections(IDictionary<string, object> dictionary)
        {
            if (dictionary == null)
                return new Dictionary<string, IRouteProjection>();

            var result = new Dictionary<string, IRouteProjection>();
            foreach (var entry in dictionary)
            {
                result.Add(entry.Key, (IRouteProjection)entry.Value);
            }
            return result;
        }
    }
}