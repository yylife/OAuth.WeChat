using Microsoft.AspNetCore.Authentication.WeChat;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public class WeChatOptions : OAuthOptions
    {
        public WeChatOptions()
        {
            AuthenticationScheme = WeChatConsts.AuthenticationScheme;
            DisplayName = AuthenticationScheme;
            CallbackPath = "/signin-WeChat";
            AuthorizationEndpoint = WeChatConsts.AuthorizationEndpoint;
            TokenEndpoint = WeChatConsts.TokenEndpoint;
            UserInformationEndpoint = WeChatConsts.UserInformationEndpoint;
        }

        public string AppId
        {
            get
            {
                return this.ClientId;
            }
            set
            {
                this.ClientId = value;
            }
        }

        public string AppSecret
        {
            get
            {
                return this.ClientSecret;
            }
            set
            {
                this.ClientSecret = value;
            }
        }
    }
}