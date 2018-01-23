using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Vault.WebApp
{
    public interface IWorkContextAccessor
    {
        WorkContext WorkContext { get; }
    }

    public class DefaultWorkContextAccessor : IWorkContextAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DefaultWorkContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));

            _httpContextAccessor = httpContextAccessor;
        }

        public WorkContext WorkContext
        {
            get
            {
                // thats fucking magic. don`t even try to use ctor injection to access to WorkContext
                // because of unexpected behavior. seriously
                return _httpContextAccessor.HttpContext
                    .RequestServices
                    .GetRequiredService<WorkContext>();
            }
        }
    }
}