using Microsoft.AspNetCore.Http;
using System;

namespace Vault.Activity.Services.Connectors
{
    public class CatchConnectionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Vault.Activity.Services.Connectors.CatchConnectionContext"/> class.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="response">Response.</param>
        ///
        public CatchConnectionContext(UserInfo user, HttpResponse response)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            User = user;
            Response = response;
        }

        /// <summary>
        /// Gets the user-specific infos
        /// </summary>
        public UserInfo User { get; }

        public HttpResponse Response { get; }
    }
}