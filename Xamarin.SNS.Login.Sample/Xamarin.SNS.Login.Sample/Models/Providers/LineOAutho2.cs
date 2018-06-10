using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;

namespace Xamarin.SNS.Login.Sample.Models.Providers
{
    public class LineOAutho2 : OAuth2Base
    {
        private static readonly Lazy<LineOAutho2> lazy = new Lazy<LineOAutho2>(() => new LineOAutho2());

        public static LineOAutho2 Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private LineOAutho2()
        {
            Initialize();
        }

        void Initialize()
        {
            ProviderName = "Line";
            Description = "Line Login Provider";
            Provider = SNSProvider.Line;
            ClientId = "Your Line Channel ID"; 
            ClientSecret = "Your Line Channel secret"; 
            Scope = "profile openid";
            AuthorizationUri = new Uri("https://access.line.me/oauth2/v2.1/authorize");
            RequestTokenUri = new Uri("https://api.line.me/oauth2/v2.1/token");
            RedirectUri = new Uri("https://github.com/imagef5"); //<- Change your redirece uri
			UserInfoUri = new Uri("https://api.line.me/v2/profile");
        }

        #region Implement Abstract Method
        public override async Task<User> GetUserInfoAsync(Account account)
        {
            User user = null;
            string token = account.Properties["access_token"];
            string refreshToke = account.Properties["refresh_token"];
            int expriesIn;
            int.TryParse(account.Properties["expires_in"], out expriesIn);

            Dictionary<string, string> dictionary = new Dictionary<string, string> { { "Authorization", token } };
            var request = new OAuth2Request("POST", UserInfoUri, dictionary, account);
            var response = await request.GetResponseAsync();
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                string userJson = await response.GetResponseTextAsync();
                var lineUser = JsonConvert.DeserializeObject<LineUser>(userJson);
                user = new User
                {
                    Id = lineUser.Id,
                    Token = token,
                    RefreshToken = refreshToke,
                    Name = lineUser.Name,
                    ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(expriesIn)),
                    PictureUrl = lineUser.ProfileImage,
                    Provider = SNSProvider.Line,
                    LoggedInWithSNSAccount = true,
                };
            }

            return user;
        }

        public override async Task<(bool IsRefresh, User User)> RefreshTokenAsync(User user)
        {
            bool refreshSuccess = false;
            //var user = AppSettings.User;
            if (user == null)
            {
                return (refreshSuccess, user);
            }

            Dictionary<string, string> dictionary = new Dictionary<string, string> { { "grant_type", "refresh_token" }, { "refresh_token", user.RefreshToken }, { "client_id", ClientId } };
            var request = new Request("POST", RequestTokenUri, dictionary, null);
            var response = await request.GetResponseAsync();
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                // Deserialize the data and store it in the account store
                // The users email address will be used to identify data in SimpleDB
                string tokenString = await response.GetResponseTextAsync();
                JObject jwtDynamic = JsonConvert.DeserializeObject<JObject>(tokenString);
                var accessToken = jwtDynamic.Value<string>("access_token");
                var refreshToken = jwtDynamic.Value<string>("refresh_token");
                var expiresIn = jwtDynamic.Value<int>("expires_in");


                user.Token = accessToken;
                user.RefreshToken = refreshToken;
                user.ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(0, 0, expiresIn));
            }

            return (refreshSuccess, user);
        }
        #endregion
    }
}
