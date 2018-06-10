using System;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace Xamarin.SNS.Login.Sample.Models.Providers
{
    public abstract class OAuth2Base
    {
        public string ProviderName { get; set; }
        public string Description { get; set; }
        public SNSProvider Provider { get; protected set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public Uri AuthorizationUri { get; set; }
        public Uri RedirectUri { get; set; }
        public Uri RequestTokenUri { get; set; }
        public Uri UserInfoUri { get; set; }

        public abstract Task<User> GetUserInfoAsync(Account account);
        public abstract Task<(bool IsRefresh, User User)> RefreshTokenAsync(User user);
    }
}
