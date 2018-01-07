using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;

namespace Vault.Shared.Authentication.Pocket
{
    public class PocketAuthenticationHandler : RemoteAuthenticationHandler<PocketOptions>
    {
        private const string StateCookie = "__PocketState";

        public PocketAuthenticationHandler(
            IOptionsMonitor<PocketOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        private HttpClient Backchannel => Options.Backchannel;

        protected override Task<object> CreateEventsAsync()
        {
            return Task.FromResult<object>(new PocketEvents());
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            if (string.IsNullOrEmpty(properties.RedirectUri))
            {
                properties.RedirectUri = CurrentUri;
            }

            var requestToken = await ObtainRequestTokenAsync(BuildRedirectUri(Options.CallbackPath), properties);

            var challengeUrl = BuildChallengeUrl(requestToken, BuildRedirectUri(Options.CallbackPath));

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps
            };

            Response.Cookies.Append(StateCookie, Options.StateDataFormat.Protect(requestToken), cookieOptions);

            var redirectContext = new PocketRedirectToAuthorizationEndpointContext(Context, Scheme, Options, properties, challengeUrl);
            await Options.Events.RedirectToAuthorizationEndpoint(redirectContext);
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            var protectedRequestToken = Request.Cookies[StateCookie];

            var cookieRequestToken = Options.StateDataFormat.Unprotect(protectedRequestToken);
            if (cookieRequestToken == null)
            {
                return HandleRequestResult.Fail("Invalid state cookie.");
            }

            var returnedRequestToken = Options.StateDataFormat.Unprotect(Request.Query["state"]);

            if (!string.Equals(returnedRequestToken.Token, cookieRequestToken.Token, StringComparison.Ordinal))
                return HandleRequestResult.Fail("Unmatched token");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps
            };

            Response.Cookies.Delete(StateCookie, cookieOptions);

            var accessToken = await ObtainAccessTokenAsync(cookieRequestToken, Options.CallbackPath);

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, accessToken.Token, ClaimValueTypes.String, Options.ClaimsIssuer),
                new Claim(ClaimTypes.Name, accessToken.ScreenName, ClaimValueTypes.String, Options.ClaimsIssuer)
            }, Options.ClaimsIssuer);
            if (Options.SaveTokens)
            {
                claimsIdentity.AddClaim(new Claim("access_token", accessToken.Token, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            return HandleRequestResult.Success(await CreateTicketAsync(claimsIdentity, cookieRequestToken.Properties, accessToken, null));
        }

        protected virtual string BuildChallengeUrl(RequestToken requestToken, string redirectUri)
        {
            string state = Options.StateDataFormat.Protect(requestToken);
            var queryBuilder = new QueryBuilder();
            queryBuilder.Add("request_token", requestToken.Token);
            queryBuilder.Add("mobile", "0");
            queryBuilder.Add("force", "login");

            var redirectQueryBuilder = new QueryBuilder();
            redirectQueryBuilder.Add("state", state);
            redirectQueryBuilder.Add("request_token", requestToken.Token);

            queryBuilder.Add("redirect_uri", redirectUri + redirectQueryBuilder);
            queryBuilder.Add("webauthenticationbroker", "0");
            return PocketDefaults.AuthorizationEndpoint + queryBuilder;
        }

        protected virtual async Task<RequestToken> ObtainRequestTokenAsync(string redirectUrl, Microsoft.AspNetCore.Authentication.AuthenticationProperties properties)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, PocketDefaults.RequestTokenEndpoint);

            request.Headers.Add("X-Accept", "application/x-www-form-urlencoded");

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "consumer_key", Options.ConsumerKey },
                        { "redirect_uri", redirectUrl }
                    });
            var response = await Backchannel.SendAsync(request, Context.RequestAborted);
            response.EnsureSuccessStatusCode();
            var formCollection = new FormCollection(new FormReader(await response.Content.ReadAsStringAsync()).ReadForm());
            var requestToken = formCollection["code"];

            return new RequestToken
            {
                CallbackConfirmed = true,
                Token = Uri.UnescapeDataString(requestToken),
                Properties = properties
            };
        }

        protected virtual Task<AuthenticationTicket> CreateTicketAsync(
           ClaimsIdentity identity, AuthenticationProperties properties, AccessToken token, JObject user)
        {
            var context = new PocketCreatingTicketContext(Context, Scheme, Options, token.UserId, token.ScreenName, token.Token, user)
            {
                Principal = new ClaimsPrincipal(identity),
                Properties = properties
            };

            // await Options.Events.CreatingTicket(context);

            if (context.Principal?.Identity == null)
            {
                return null;
            }

            return Task.FromResult(
                new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name));
        }

        protected virtual async Task<AccessToken> ObtainAccessTokenAsync(RequestToken requestToken, string redirectUri)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        {
                            "consumer_key",
                            this.Options.ConsumerKey
                        },
                        {
                            "redirect_uri",
                            redirectUri
                        },
                        {
                            "code",
                            requestToken.Token
                        }
                    });
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, PocketDefaults.AccessTokenEndpoint);
            request.Headers.Add("X-Accept", "application/json");
            request.Content = content;
            HttpResponseMessage response = await Backchannel.SendAsync(request, this.Context.RequestAborted);
            response.EnsureSuccessStatusCode();
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            return new AccessToken
            {
                Token = result.GetValue("access_token").Value<string>(),
                ScreenName = result.GetValue("username").Value<string>()
            };
        }
    }
}