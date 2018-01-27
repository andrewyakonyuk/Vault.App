using System;
using Microsoft.AspNetCore.Http;
using Vault.WebApp.Infrastructure.Identity;

namespace Vault.WebApp
{
    public class WorkContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WorkContext(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));

            _httpContextAccessor = httpContextAccessor;
        }

        public IUser Owner { get; set; }

        public IUser User { get; set; }

        public HttpContext HttpContext { get { return _httpContextAccessor.HttpContext; } }
    }
}