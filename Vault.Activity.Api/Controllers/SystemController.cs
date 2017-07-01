using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Vault.Activity.Services.Connectors;
using Vault.Shared.Activity;

namespace Vault.Activity.Api.Controllers
{
    [Route("api/[controller]")]
    public class SystemController : Controller
    {
        readonly IConnectionPool<IPullConnectionProvider> _connectionPool;
        readonly ILoggerFactory _loggerFactory;
        readonly IClock _clock;
        readonly IActivityClient _activityClient;

        public SystemController(
            IConnectionPool<IPullConnectionProvider> connectionPull,
            ILoggerFactory loggerFactory,
            IClock clock,
            IActivityClient client)
        {
            _connectionPool = connectionPull;
            _loggerFactory = loggerFactory;
            _clock = clock;
            _activityClient = client;
        }

        [HttpPost("pull")]
        public async Task<IActionResult> Pull(
           [FromBody] ExternalLoginInfo loginInfo)
        {
            var worker = new PollingConnectionWorker(loginInfo.ProviderName, loginInfo.ProviderKey, loginInfo.OwnerId,
                _connectionPool, _activityClient,
                _loggerFactory.CreateLogger<PollingConnectionWorker>(), _clock);
            await worker.PullAsync();

            return Ok();
        }
    }

    public class ExternalLoginInfo
    {
        [Required]
        public string ProviderName { get; set; }

        [Required]
        public string ProviderKey { get; set; }

        [Required]
        public string OwnerId { get; set; }
    }
}
