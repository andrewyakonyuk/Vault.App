using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;

namespace Vault.Shared.Authentication.Pocket
{
    public class PocketAuthenticationHandler : RemoteAuthenticationHandler<PocketOptions>
    {
        private readonly HttpClient _httpClient;
        private const string StateCookie = "__PocketState";

        public PocketAuthenticationHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected override async Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var properties = new AuthenticationProperties(context.Properties);

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

            var redirectContext = new PocketRedirectToAuthorizationEndpointContext(Context, Options, properties, challengeUrl);
            await Options.Events.RedirectToAuthorizationEndpoint(redirectContext);

            return true;
        }

        protected override async Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            var protectedRequestToken = Request.Cookies[StateCookie];

            var cookieRequestToken = Options.StateDataFormat.Unprotect(protectedRequestToken);
            if (cookieRequestToken == null)
            {
                return AuthenticateResult.Fail("Invalid state cookie.");
            }

            var returnedRequestToken = Options.StateDataFormat.Unprotect(Request.Query["state"]);

            if (!string.Equals(returnedRequestToken.Token, cookieRequestToken.Token, StringComparison.Ordinal))
                return AuthenticateResult.Fail("Unmatched token");

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

            return AuthenticateResult.Success(await CreateTicketAsync(claimsIdentity, cookieRequestToken.Properties, accessToken, null));
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

            queryBuilder.Add("redirect_uri", redirectUri + redirectQueryBuilder.ToString());
            queryBuilder.Add("webauthenticationbroker", "0");
            return PocketDefaults.AuthorizationEndpoint + queryBuilder.ToString();
        }

        protected virtual async Task<RequestToken> ObtainRequestTokenAsync(string redirectUrl, AuthenticationProperties properties)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, PocketDefaults.RequestTokenEndpoint);

            request.Headers.Add("X-Accept", "application/x-www-form-urlencoded");

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "consumer_key", Options.ConsumerKey },
                        { "redirect_uri", redirectUrl }
                    });
            var response = await _httpClient.SendAsync(request, Context.RequestAborted);
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

        protected virtual async Task<AuthenticationTicket> CreateTicketAsync(
           ClaimsIdentity identity, AuthenticationProperties properties, AccessToken token, JObject user)
        {
            var context = new PocketCreatingTicketContext(Context, Options, token.UserId, token.ScreenName, token.Token, user)
            {
                Principal = new ClaimsPrincipal(identity),
                Properties = properties
            };

            // await Options.Events.CreatingTicket(context);

            if (context.Principal?.Identity == null)
            {
                return null;
            }

            return new AuthenticationTicket(context.Principal, context.Properties, Options.AuthenticationScheme);
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
            HttpResponseMessage response = await _httpClient.SendAsync(request, this.Context.RequestAborted);
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