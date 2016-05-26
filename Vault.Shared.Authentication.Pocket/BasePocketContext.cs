using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Http;

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