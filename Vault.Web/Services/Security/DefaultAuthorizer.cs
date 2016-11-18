using Vault.Shared;

namespace Vault.WebHost.Services.Security
{
    public class DefaultAuthorizer : IAuthorizer
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger _logger;
        private readonly IWorkContextAccessor _workContextAccessor;

        public DefaultAuthorizer(
            IAuthorizationService authorizationService,
            ILogger logger,
            IWorkContextAccessor workContextAccessor)
        {
            _authorizationService = authorizationService;
            _logger = logger;
            _workContextAccessor = workContextAccessor;
        }

        public bool Authorize(Permission permission)
        {
            return Authorize(permission, null);
        }

        public bool Authorize(Permission permission, IContent content)
        {
            if (_authorizationService.TryCheckAccess(permission, _workContextAccessor.WorkContext.User, content))
                return true;

            if (_workContextAccessor.WorkContext.User == null)
            {
                _logger.WriteInfo("Anonymous users do not have {0} permission.", permission.Name);
            }
            else
            {
                _logger.WriteInfo("Current user, {1}, does not have {0} permission.", permission.Name, _workContextAccessor.WorkContext.User.UserName);
            }

            return false;
        }
    }
}