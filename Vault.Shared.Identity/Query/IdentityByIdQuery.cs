using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared.Queries;

namespace Vault.Shared.Identity.Query
{
    public class IdentityByIdQuery : IQuery<FindById, IdentityUser>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityByIdQuery(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityUser> AskAsync(FindById criterion)
        {
            if (criterion == null)
                throw new ArgumentNullException(nameof(criterion));

            return await _userManager.FindByIdAsync(criterion.Id.ToString(CultureInfo.InvariantCulture));
        }
    }
}