using System;
using Microsoft.AspNetCore.Authentication.WeChat;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods to add WeChat authentication capabilities to an HTTP application pipeline.
    /// </summary>
    public static class WeChatAppBuilderExtensions
    {
        public static IApplicationBuilder UseWeChatAuthentication(this IApplicationBuilder app, WeChatOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<WeChatMiddleware>(options);
        }

        public static IApplicationBuilder UseWeChatAuthentication(this IApplicationBuilder app, Action<WeChatOptions> configureOptions)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var options = new WeChatOptions();
            configureOptions?.Invoke(options);

            return app.UseWeChatAuthentication(options);
        }
    }
}
