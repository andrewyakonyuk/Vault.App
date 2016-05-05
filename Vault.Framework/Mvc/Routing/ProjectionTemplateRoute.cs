using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vault.Framework.Mvc.Routing
{
    public class ProjectionTemplateRoute : TemplateRoute
    {
        private readonly IReadOnlyDictionary<string, IRouteProjection> _projections;

        public IReadOnlyDictionary<string, IRouteProjection> Projections { get { return _projections; } }

        public ProjectionTemplateRoute(
            IRouter target,
            string routeTemplate,
            IInlineConstraintResolver inlineConstraintResolver)
            : this(target, routeTemplate, null, null, null, null, inlineConstraintResolver)
        {
        }

        public ProjectionTemplateRoute(
            IRouter target,
            string routeTemplate,
            IDictionary<string, object> defaults,
            IDictionary<string, object> constraints,
            IDictionary<string, object> projections,
            IDictionary<string, object> dataTokens,
            IInlineConstraintResolver inlineConstraintResolver)
            : this(target, null, routeTemplate, defaults, constraints, projections, dataTokens, inlineConstraintResolver)
        {
        }

        public ProjectionTemplateRoute(
            IRouter target,
            string routeName,
            string routeTemplate,
            IDictionary<string, object> defaults,
            IDictionary<string, object> constraints,
            IDictionary<string, object> projections,
            IDictionary<string, object> dataTokens,
            IInlineConstraintResolver inlineConstraintResolver)
                : base(target,
                routeName,
                routeTemplate,
                defaults,
                constraints,
                dataTokens,
                inlineConstraintResolver)
        {
            _projections = GetProjections(projections);
        }

        public override VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            foreach (var item in Projections)
            {
                if (item.Value == null)
                    continue;

                item.Value.Outgoing(item.Key, context.Values);
            }

            return base.GetVirtualPath(context);
        }

        public override Task RouteAsync(RouteContext context)
        {
            foreach (var item in Projections)
            {
                if (item.Value == null)
                    continue;

                item.Value.Incoming(item.Key, context.RouteData.Values);
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