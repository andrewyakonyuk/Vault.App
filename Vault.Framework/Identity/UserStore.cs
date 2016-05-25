using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vault.Shared.Domain;
using Vault.Shared.NHibernate;
using System.Collections.Generic;

namespace Vault.Framework.Identity
{
    public class UserStore : IUserStore<IdentityUser>, IUserPasswordStore<IdentityUser>, IUserEmailStore<IdentityUser>, IUserLoginStore<IdentityUser>
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ILinqProvider _linqProvider;

        public UserStore(IUnitOfWorkFactory uowFactory, ILinqProvider linqProvider)
        {
            _uowFactory = uowFactory;
            _linqProvider = linqProvider;
        }

        public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            using (var uow = _uowFactory.Create())
            {
                uow.Save(user);
                uow.Commit();
                return Task.FromResult(IdentityResult.Success);
            }
        }

        public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            using (var uow = _uowFactory.Create(System.Data.IsolationLevel.ReadUncommitted))
            {
                uow.Delete(user);
                uow.Commit();
                return Task.FromResult(IdentityResult.Success);
            }
        }

        public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            using (var uow = _uowFactory.Create())
            {
                uow.Save(user);
                uow.Commit();
                return Task.FromResult(IdentityResult.Success);
            }
        }

        public void Dispose()
        {
        }

        public Task<IdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var result = _linqProvider.Query<IdentityUser>()
               .Where(t => t.NormalizedEmail == normalizedEmail).FirstOrDefault();

            return Task.FromResult(result);
        }

        public Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var result = _linqProvider.Query<IdentityUser>()
               .Where(t => t.Id.ToString() == userId).FirstOrDefault();

            return Task.FromResult(result);
        }

        public Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var result = _linqProvider.Query<IdentityUser>()
                .Where(t => t.NormalizedUserName == normalizedUserName).FirstOrDefault();

            return Task.FromResult<IdentityUser>(result);
        }

        public Task<string> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetEmailAsync(IdentityUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            user.Email = email;
            return Task.FromResult(true);
        }

        public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            user.EmailConfirmed = true;
            return Task.FromResult(true);
        }

        public Task SetNormalizedEmailAsync(IdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(true);
        }

        public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            user.NormalizedUserName = normalizedName;
            return Task.FromResult(true);
        }

        public Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            user.PasswordHash = passwordHash;
            return Task.FromResult(true);
        }

        public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            user.UserName = userName;
            return Task.FromResult(true);
        }

        public Task AddLoginAsync(IdentityUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            user.Logins.Add(new IdentityUserLogin
            {
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
                ProviderKey = login.ProviderKey
            });

            return Task.FromResult(true);
        }

        public Task RemoveLoginAsync(IdentityUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var login = user.Logins.SingleOrDefault(t => t.LoginProvider == loginProvider && t.ProviderKey == providerKey);
            if (login != null)
            {
                user.Logins.Remove(login);
            }

            return Task.FromResult(true);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return user.Logins.Select(t => new UserLoginInfo(t.LoginProvider, t.ProviderKey, t.ProviderDisplayName)).ToArray();
        }

        public Task<IdentityUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var login = _linqProvider.Query<IdentityUserLogin>()
                .Where(t => t.LoginProvider == loginProvider && t.ProviderKey == providerKey)
                .FirstOrDefault();

            if (login == null)
                return Task.FromResult<IdentityUser>(null);

            return Task.FromResult(login.User);
        }
    }
}