using Microsoft.AspNetCore.Http;
using System;
using Vault.Shared.Identity;

namespace Vault.Framework
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