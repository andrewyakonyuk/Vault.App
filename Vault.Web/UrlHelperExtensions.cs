using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Web
{
    public static class UrlHelperExtensions
    {
        public static string Board(this IUrlHelper urlHelper, int boardId, string title, string username)
        {
            return urlHelper.RouteUrl(new UrlRouteContext
            {
                RouteName = "board-detail",
                Values = new
                {
                    boardId,
                    title,
                    username
                }
            });
        }
    }
}