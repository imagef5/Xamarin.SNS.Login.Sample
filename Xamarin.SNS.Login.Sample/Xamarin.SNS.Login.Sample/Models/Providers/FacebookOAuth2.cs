using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;

namespace Xamarin.SNS.Login.Sample.Models.Providers
{
    public class FacebookOAuth2 : OAuth2Base
    {
        private static readonly Lazy<FacebookOAuth2> lazy = new Lazy<FacebookOAuth2>(() => new FacebookOAuth2());

        public static FacebookOAuth2 Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private FacebookOAuth2()
        {
            Initialize();
        }

        void Initialize()
        {
            ProviderName = "Facebook";
            Description = "Facebook Login Provider";
            Provider = SNSProvider.Facebook;
            ClientId = "Your facebook Client Id";
            ClientSecret = "Your facebook Client Secret";
            Scope = "email";
            AuthorizationUri = new Uri("https://www.facebook.com/dialog/oauth");
            RequestTokenUri = new Uri("https://graph.facebook.com/oauth/access_token");
            RedirectUri = new Uri("https://www.facebook.com/connect/login_success.html");
            UserInfoUri = new Uri("https://graph.facebook.com/me"); //access_token=...&client_secret=...&redirect_uri=...&client_id=...
        }

        #region Implement Abstract Method
        public override async Task<User> GetUserInfoAsync(Account account)
        {
            User user = null;
            string token = account.Properties["access_token"];
            int expriesIn;
            int.TryParse(account.Properties["expires_in"], out expriesIn);


            Dictionary<string, string> dictionary = new Dictionary<string, string> { { "fields", "name,email,picture,first_name,last_name" } };
            var request = new OAuth2Request("GET", UserInfoUri, dictionary, account);
            var response = await request.GetResponseAsync();
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                string userJson = await response.GetResponseTextAsync();
                var facebookUser = JsonConvert.DeserializeObject<FacebookUser>(userJson);
                user = new User
                {
                    Id = facebookUser.Id,
                    Token = token,
                    RefreshToken = null,
                    Name = facebookUser.Name,
                    Email = facebookUser.Email,
                    ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(expriesIn)),
                    PictureUrl = facebookUser.Picture.Data.Url,
                    Provider = SNSProvider.Facebook,
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

            //https://graph.facebook.com/oauth/client_code?access_token=...&client_secret=...&redirect_uri=...&client_id=...
            //-> return {code : ""}
            string code = null;
            Uri codeUri = new Uri($"https://graph.facebook.com/oauth/client_code?access_token={user.Token}&client_secret={ClientSecret}&redirect_uri={RedirectUri.AbsoluteUri}&client_id={ClientId}");

            var request = new Request("POST", codeUri, null, null);
            var response = await request.GetResponseAsync();
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                string tokenString = await response.GetResponseTextAsync();
                JObject jwtDynamic = JsonConvert.DeserializeObject<JObject>(tokenString);
                code = jwtDynamic.Value<string>("code");
            }

            if (!string.IsNullOrEmpty(code))
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string> { { "code", code }, { "client_id", ClientId }, { "redirect_uri", RedirectUri.AbsoluteUri } };
                var refreshRequest = new Request("POST", RequestTokenUri, dictionary, null);
                var refreshResponse = await refreshRequest.GetResponseAsync();
                if (refreshResponse != null && refreshResponse.StatusCode == HttpStatusCode.OK)
                {
                    string tokenString = await refreshResponse.GetResponseTextAsync();
                    JObject jwtDynamic = JsonConvert.DeserializeObject<JObject>(tokenString);
                    var accessToken = jwtDynamic.Value<string>("access_token");
                    var expiresIn = jwtDynamic.Value<int>("expires_in");


                    user.Token = accessToken;
                    user.ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(0, 0, expiresIn));

                    refreshSuccess = true;
                }
            }
            //machine_id <- option
            //https://graph.facebook.com/oauth/access_token?code=...&client_id=...&redirect_uri=...&machine_id= ...
            //-> return {"access_token":"...", "expires_in":..., "machine_id":"..."}
            return (refreshSuccess, user);
        }
        #endregion
    }
}
