using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.Authentication.WeChat
{
    public class WeChatHandler : OAuthHandler<WeChatOptions>
    {
        public WeChatHandler(HttpClient httpClient) : base(httpClient)
        {
        }

        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
        {
            HttpResponseMessage async =
                await this.Backchannel.GetAsync(this.Options.TokenEndpoint + (object)new QueryBuilder()
                                                {
                                                    {
                                                        "appid",
                                                        this.Options.AppId
                                                    },
                                                    {
                                                        "secret",
                                                        this.Options.AppSecret
                                                    },
                                                    {
                                                        "code",
                                                        code
                                                    },
                                                    {
                                                        "grant_type",
                                                        "authorization_code"
                                                    },
                                                    {
                                                        "redirect_uri",
                                                        redirectUri
                                                    }
                                                }, this.Context.RequestAborted);
            async.EnsureSuccessStatusCode();
            return OAuthTokenResponse.Success(JObject.Parse(await async.Content.ReadAsStringAsync()));
        }



        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity,
            AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            string id = WeChatHelper.GetId(tokens.Response);
            if (!string.IsNullOrEmpty(id))
            {
                identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", id,
                    "http://www.w3.org/2001/XMLSchema#string", this.Options.ClaimsIssuer));
                identity.AddClaim(new Claim("urn:wechat:id", id, "http://www.w3.org/2001/XMLSchema#string",
                    this.Options.ClaimsIssuer));
            }
            HttpResponseMessage async =
                await this.Backchannel.GetAsync(this.Options.UserInformationEndpoint + (object)new QueryBuilder()
                                                {
                                                    {
                                                        "access_token",
                                                        tokens.AccessToken
                                                    },
                                                    {
                                                        "openid",
                                                        id
                                                    }
                                                }, this.Context.RequestAborted);
            async.EnsureSuccessStatusCode();
            JObject jobject = JObject.Parse(await async.Content.ReadAsStringAsync());

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), properties,
                Options.AuthenticationScheme);

            var notification = new OAuthCreatingTicketContext(ticket, this.Context,
                this.Options, this.Backchannel, tokens, jobject);
            string name = WeChatHelper.GetName(jobject);
            if (!string.IsNullOrEmpty(name))
            {
                identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", name,
                    "http://www.w3.org/2001/XMLSchema#string", this.Options.ClaimsIssuer));
                identity.AddClaim(new Claim("urn:wechat:name", name, "http://www.w3.org/2001/XMLSchema#string",
                    this.Options.ClaimsIssuer));
            }
            string headImage = WeChatHelper.GetHeadImage(jobject);
            if (!string.IsNullOrEmpty(headImage))
                identity.AddClaim(new Claim("urn:wechat:headimgurl", headImage,
                    "http://www.w3.org/2001/XMLSchema#string", this.Options.ClaimsIssuer));
            await this.Options.Events.CreatingTicket(notification);
            return notification.Ticket;
        }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            return this.Options.AuthorizationEndpoint + (object)new QueryBuilder()
                   {
                       {
                           "appid",
                           this.Options.AppId
                       },
                       {
                           "redirect_uri",
                           redirectUri
                       },
                       {
                           "response_type",
                           "code"
                       },
                       {
                           "scope",
                           this.FormatScope()
                       },
                       {
                           "state",
                           this.Options.StateDataFormat.Protect(properties)
                       }
                   };
        }

        private static void AddQueryString(IDictionary<string, string> queryStrings, AuthenticationProperties properties,
            string name, string defaultValue = null)
        {
            string value;
            if (!properties.Items.TryGetValue(name, out value))
            {
                value = defaultValue;
            }
            else
            {
                // Remove the parameter from AuthenticationProperties so it won't be serialized to state parameter
                properties.Items.Remove(name);
            }

            if (value == null)
            {
                return;
            }

            queryStrings[name] = value;
        }

        protected override string FormatScope()
        {
            return string.Join(",", this.Options.Scope);
        }
    }
}