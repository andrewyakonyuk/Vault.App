using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using Vault.Shared.Queries;

namespace Vault.Framework.Identity.Query
{
    public class FindByUsernameQuery : IQuery<Username, IdentityUser>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public FindByUsernameQuery(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityUser> AskAsync(Username criterion)
        {
            if (criterion == null)
                throw new ArgumentNullException(nameof(criterion));

            return await _userManager.FindByNameAsync(criterion.Value);
        }
    }
}