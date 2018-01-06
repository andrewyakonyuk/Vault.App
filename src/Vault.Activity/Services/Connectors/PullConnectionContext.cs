using System;
using System.Threading;

namespace Vault.Activity.Services.Connectors
{
    public class PullConnectionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Vault.Activity.Services.Connectors.PullConnectionContext"/> class.
        /// </summary>
        /// <param name="user">User.</param>
        public PullConnectionContext(UserInfo user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            User = user;
        }

        /// <summary>
        /// Gets or sets a batch number to fetch from external data source
        /// </summary>
        public int Batch { get; set; }

        /// <summary>
        /// Gets the user-specific infos
        /// </summary>
        public UserInfo User { get; }

        /// <summary>
        /// Gets or sets the last fetch time of data from the source in UTC date format
        /// </summary>
        public DateTimeOffset? LastFetchTimeUtc { get; set; }
    }
}