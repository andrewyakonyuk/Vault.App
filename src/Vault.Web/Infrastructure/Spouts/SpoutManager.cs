using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StreamInsights.Abstractions;
using Vault.Spouts.Abstractions;
using Vault.WebApp.Infrastructure.Identity;
using SActivity = StreamInsights.Abstractions.Activity;

namespace Vault.WebApp.Infrastructure.Spouts
{
    public class SpoutManager
    {
        readonly IServiceProvider _serviceProvider;
        readonly UserManager<IdentityUser> _userManager;
        readonly IActivityClient _activityClient;
        readonly SpoutOptions _options;

        public SpoutManager(
            IOptions<SpoutOptions> options,
            IServiceProvider serviceProvider,
            UserManager<IdentityUser> userManager,
            IActivityClient activityClient)
        {
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _activityClient = activityClient;
            _options = options.Value;
        }

        public async Task ConsumeAsync(string providerName, string providerKey)
        {
            if (!_options.Services.TryGetValue(providerName, out Type spoutType))
                throw new NotSupportedException();

            var spout = (ISpout<SActivity>)ActivatorUtilities.CreateInstance(_serviceProvider, spoutType);

            var user = await _userManager.FindByLoginAsync(providerName, providerKey);
            if (user == null)
                return;

            var userInfo = new UserInfo(providerKey, user.Id.ToString());

            //todo: get context from storage
            var context = new ConsumeMessageContext(userInfo);

            ConsumeResult<SActivity> result = null;
            var batch = 0;
            var stream = _activityClient.GetStream("default", user.Id.ToString());
            do
            {
                context.Batch = batch;
                result = await spout.ConsumeAsync(context);

                foreach (var activity in result)
                {
                    await stream.PushActivityAsync(activity);
                }

                batch++;
            } while (!result.IsCancellationRequested);

            //todo: save context
        }
    }
}
