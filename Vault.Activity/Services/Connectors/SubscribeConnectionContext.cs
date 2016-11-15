using System;

namespace Vault.Activity.Services.Connectors
{
    public class SubscribeConnectionContext
    {
        public SubscribeConnectionContext(UserInfo user, Guid subscriptionId)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            User = user;
            SubscriptionId = subscriptionId;
        }

        public UserInfo User { get; }

        public Guid SubscriptionId { get; }
    }
}