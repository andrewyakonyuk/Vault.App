using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Vault.Shared.Domain;
using Vault.Shared.NHibernate;
using Vault.WebApp.Infrastructure.Persistence;
using Dapper;
using System.Data;

namespace Vault.WebApp.Infrastructure.Identity
{
    public class UserStore : IUserStore<IdentityUser>, IUserPasswordStore<IdentityUser>, IUserEmailStore<IdentityUser>, IUserLoginStore<IdentityUser>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public UserStore(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        #region IUserStore
        public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            using (var connection = _dbConnectionFactory.Create())
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                user.ConcurrencyStamp = Guid.NewGuid().ToString("N");
                await connection.ExecuteAsync(new CommandDefinition(
                    @"INSERT INTO identity_user (
                        concurrency_stamp, email, email_confirmed,
                        normalized_email, normalized_user_name, password_hash, user_name) 
                    VALUES (
                        @concurrencyStamp, @email, @emailConfirmed,
                        @normilizedEmail, @normalizedUserName, @passwordHash, @username);",
                    user,
                    transaction,
                    cancellationToken: cancellationToken));
                transaction.Commit();
                return IdentityResult.Success;
            }
        }

        public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            using (var connection = _dbConnectionFactory.Create())
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                await connection.ExecuteAsync(new CommandDefinition(
                    @"DELETE FROM identity_user
	                WHERE identity_user_id = @id;", new { id = user.Id },
                    transaction,
                    cancellationToken: cancellationToken));

                transaction.Commit();
                return IdentityResult.Success;
            }
        }

        public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            using (var connection = _dbConnectionFactory.Create())
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var newConcurrencyStamp = Guid.NewGuid().ToString("N");

                await connection.ExecuteAsync(new CommandDefinition(
                    $@"UPDATE identity_user
	                    SET identity_user_id = @id, 
                            concurrency_stamp = '{newConcurrencyStamp}', 
                            email = @email, 
                            email_confirmed = @emailConfirmed, 
                            normalized_email = @normalizedEmail, 
                            normalized_user_name = @normalizedUsername, 
                            password_hash = @passwordHash, 
                            user_name = @username
	                    WHERE identity_user_id = @id
                            AND concurrency_stamp = @concurrencyStamp;",
                    user,
                    transaction,
                    cancellationToken: cancellationToken));
                transaction.Commit();

                return IdentityResult.Success;
            }
        }

        public async Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            using (var connection = _dbConnectionFactory.Create())
            {
                var result = await connection.QueryFirstAsync<IdentityUser>(
                    new CommandDefinition(
                        @"SELECT identity_user_id as id, 
                            concurrency_stamp as concurrencyStamp, 
                            email, 
                            email_confirmed as emailConfirmed, 
                            normalized_email as normalizedEmail, 
                            normalized_user_name as normalizedUsername, 
                            password_hash as passwordHash,  
                            user_name as username
	                    FROM identity_user
                        WHERE identity_user_id = @userId;",
                        new { userId = Convert.ToInt32(userId) },
                        cancellationToken: cancellationToken));

                return result;
            }
        }

        public async Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedUserName))
                throw new ArgumentNullException(nameof(normalizedUserName));
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = _dbConnectionFactory.Create())
            {
                var result = await connection.QueryFirstAsync<IdentityUser>(
                    new CommandDefinition(
                       @"SELECT identity_user_id as id, 
                            concurrency_stamp as concurrencyStamp, 
                            email, 
                            email_confirmed as emailConfirmed, 
                            normalized_email as normalizedEmail, 
                            normalized_user_name as normalizedUsername, 
                            password_hash as passwordHash, 
                            user_name as username
	                    FROM identity_user
                        WHERE normalized_user_name = @normalizedUserName;",
                        new { normalizedUserName },
                        cancellationToken: cancellationToken));

                return result;
            }
        }

        public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedUserName = normalizedName;
            return Task.FromResult(true);
        }

        public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.UserName = userName;
            return Task.FromResult(true);
        }

        #endregion

        #region IUserEmailStore
        public async Task<IdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            using (var connection = _dbConnectionFactory.Create())
            {
                var result = await connection.QueryFirstAsync<IdentityUser>(
                    new CommandDefinition(
                        @"SELECT identity_user_id as id, 
                            concurrency_stamp as concurrencyStamp, 
                            email, 
                            email_confirmed as emailConfirmed, 
                            normalized_email as normalizedEmail, 
                            normalized_user_name as normalizedUsername, 
                            password_hash as passwordHash, 
                            user_name as username
	                    FROM identity_user
                        WHERE normalized_email = @normalizedEmail;",
                        new { normalizedEmail },
                        cancellationToken: cancellationToken));

                return result;
            }
        }

        public Task<string> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetEmailAsync(IdentityUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.Email = email;
            return Task.FromResult(true);
        }

        public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.EmailConfirmed = true;
            return Task.FromResult(true);
        }

        public Task SetNormalizedEmailAsync(IdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(true);
        }

        #endregion

        #region IUserPasswordStore
        public Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PasswordHash = passwordHash;
            return Task.FromResult(true);
        }
        #endregion

        #region IUserLoginStore
        public async Task AddLoginAsync(IdentityUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (login == null)
                throw new ArgumentNullException(nameof(login));
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = _dbConnectionFactory.Create())
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                await connection.ExecuteAsync(new CommandDefinition(
                    @"INSERT INTO identity_user_login (
	                    login_provider, provider_display_name, provider_key, user_id)
	                    VALUES (@loginProvider, @displayName, @providerKey, @userId);",
                    new
                    {
                        loginProvider = login.LoginProvider,
                        displayName = login.ProviderDisplayName,
                        providerKey = login.ProviderKey,
                        userId = user.Id
                    },
                    transaction,
                    cancellationToken: cancellationToken
                    ));

                transaction.Commit();
            }
        }

        public async Task RemoveLoginAsync(IdentityUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));
            if (string.IsNullOrEmpty(providerKey))
                throw new ArgumentNullException(nameof(providerKey));
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = _dbConnectionFactory.Create())
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        @"DELETE FROM identity_user_login
	                    WHERE 
                            login_provider = @loginProvider
                            AND provider_key = @providerKey
                            AND user_id = @userId",
                        new { loginProvider, providerKey, userId = user.Id },
                        transaction,
                        cancellationToken: cancellationToken));
                transaction.Commit();
            }
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = _dbConnectionFactory.Create())
            {
                var result = await connection.QueryAsync<UserLoginInfo>(
                    new CommandDefinition(
                        @"SELECT login_provider as loginprovider, 
                            provider_key as providerkey,
                            provider_display_name as displayname 
	                    FROM identity_user_login
                        WHERE user_id = @id;",
                        new { id = user.Id },
                        cancellationToken: cancellationToken
                    ));

                return result.ToArray();
            }
        }

        public async Task<IdentityUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            using (var connection = _dbConnectionFactory.Create())
            {
                var result = await connection.QuerySingleOrDefaultAsync<IdentityUser>(
                    new CommandDefinition(
                        @"SELECT u.identity_user_id as id, 
                            u.concurrency_stamp as concurrencyStamp, 
                            u.email, 
                            u.email_confirmed as emailConfirmed, 
                            u.normalized_email as normalizedEmail, 
                            u.normalized_user_name as normalizedUsername, 
                            u.password_hash as passwordHash, 
                            u.user_name as username
	                    FROM identity_user_login l
                        INNER JOIN identity_user u ON l.user_id = u.identity_user_id
                        WHERE login_provider = @loginProvider
                            AND provider_key = @providerKey",
                        new { loginProvider, providerKey },
                        cancellationToken: cancellationToken));

                return result;
            }
        }
        #endregion

        public void Dispose()
        {
        }
    }
}