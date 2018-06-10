using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;

namespace Xamarin.SNS.Login.Sample.Models.Providers
{
    public class KakaoOAutho2 : OAuth2Base
    {
        private static readonly Lazy<KakaoOAutho2> lazy = new Lazy<KakaoOAutho2>(() => new KakaoOAutho2());

        public static KakaoOAutho2 Instance 
        { 
            get 
            { 
                return lazy.Value; 
            } 
        }

        private KakaoOAutho2()
        {
            Initialize();
        }

        void Initialize()
        {
            ProviderName = "Kakao";
            Description = "Kakao Login Provider";
            Provider = SNSProvider.Kakao;
            ClientId = "Your Kakao Api Key"; 
            ClientSecret = null;
            Scope = null;
            AuthorizationUri = new Uri("https://kauth.kakao.com/oauth/authorize");
            RequestTokenUri = new Uri("https://kauth.kakao.com/oauth/token");
            RedirectUri = new Uri("https://github.com/imagef5"); // <- Change your redirece uri
            UserInfoUri = new Uri("https://kapi.kakao.com/v1/user/me");
        }

        #region Implement Abstract Method
        public override async Task<User> GetUserInfoAsync(Account account)
        {
            User user = null;
            string token = account.Properties["access_token"];
            string refreshToke = account.Properties["refresh_token"];
            int expriesIn;
            int.TryParse(account.Properties["expires_in"], out expriesIn);

            var request = new OAuth2Request("GET", UserInfoUri, null, account);
            var response = await request.GetResponseAsync();
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                string userJson = await response.GetResponseTextAsync();
                var kakaoUser = JsonConvert.DeserializeObject<KakaoUser>(userJson);
                user = new User
                {
                    Id = kakaoUser.Id,
                    Token = token,
                    RefreshToken = refreshToke,
                    Name = kakaoUser.Properties.NickName,
                    Email = kakaoUser.Email,
                    ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(expriesIn)),
                    PictureUrl = kakaoUser.Properties.ProfileImage,
                    Provider = SNSProvider.Kakao,
                    LoggedInWithSNSAccount = true,
                };
            }
            //AppSettings.User = user;
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
                string tokenString = await response.GetResponseTextAsync();
                JObject jwtDynamic = JsonConvert.DeserializeObject<JObject>(tokenString);
                var accessToken = jwtDynamic.Value<string>("access_token");
                var refreshToken = jwtDynamic.Value<string>("refresh_token");
                var expiresIn = jwtDynamic.Value<int>("expires_in");


                user.Token = accessToken;
                user.RefreshToken = refreshToken;
                user.ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(0,0,expiresIn));

                refreshSuccess = true;
            }

            return (refreshSuccess, user);
        }
        #endregion
    }
}
