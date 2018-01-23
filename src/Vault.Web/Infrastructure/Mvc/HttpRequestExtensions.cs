using System;
using Microsoft.AspNetCore.Http;

namespace Vault.WebHost.Infrastructure.Mvc
{
    public static class HttpRequestExtensions
    {
        public static bool IsPjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            return request.Headers != null && !string.IsNullOrEmpty(request.Headers["X-PJAX"]);
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }
    }
}