using System;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.Authentication.WeChat
{
    public class WeChatHelper
    {
        internal static string GetId(JObject payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            return payload.Value<string>((object)"openid");
        }

        internal static string GetName(JObject payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            return payload.Value<string>((object)"nickname");
        }

        internal static string GetHeadImage(JObject payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            return payload.Value<string>((object)"headimgurl");
        }
    }
}