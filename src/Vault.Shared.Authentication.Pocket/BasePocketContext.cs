using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Vault.Shared.Authentication.Pocket
{
    public class BasePocketContext : BaseContext
    {
        public BasePocketContext(HttpContext context, PocketOptions options)
            : base(context)
        {
            Options = options;
        }

        public PocketOptions Options { get; private set; }
    }
}