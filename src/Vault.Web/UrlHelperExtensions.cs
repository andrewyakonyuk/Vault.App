using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Vault.WebHost
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