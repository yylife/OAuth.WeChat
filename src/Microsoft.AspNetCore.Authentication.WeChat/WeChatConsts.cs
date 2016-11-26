namespace Microsoft.AspNetCore.Authentication.WeChat
{
    public class WeChatConsts
    {
        public const string AuthenticationScheme = "WeChat";

        public const string AuthorizationEndpoint = "https://open.weixin.qq.com/connect/qrconnect";

        public const string TokenEndpoint = "https://api.weixin.qq.com/sns/oauth2/access_token";

        public const string UserInformationEndpoint = "https://api.weixin.qq.com/sns/userinfo";
    }
}