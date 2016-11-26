using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Authentication.WeChat
{
    public class WeChatMiddleware : OAuthMiddleware<WeChatOptions>
    {
        public WeChatMiddleware(RequestDelegate next, IDataProtectionProvider dataProtectionProvider,
            ILoggerFactory loggerFactory, UrlEncoder encoder, IOptions<SharedAuthenticationOptions> sharedOptions,
            IOptions<WeChatOptions> options)
            : base(next, dataProtectionProvider, loggerFactory, encoder, sharedOptions, options)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (dataProtectionProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }

            if (sharedOptions == null)
            {
                throw new ArgumentNullException(nameof(sharedOptions));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrWhiteSpace(this.Options.AppId))
                throw new ArgumentException("AppId");

            if (string.IsNullOrWhiteSpace(this.Options.AppSecret))
                throw new ArgumentException("AppSecret");

            if (Options.Scope.Count == 0)
            {
                this.Options.Scope.Add("snsapi_login");
                this.Options.Scope.Add("snsapi_userinfo");
            }
        }

        protected override AuthenticationHandler<WeChatOptions> CreateHandler()
        {
            return new WeChatHandler(Backchannel);
        }
    }
}