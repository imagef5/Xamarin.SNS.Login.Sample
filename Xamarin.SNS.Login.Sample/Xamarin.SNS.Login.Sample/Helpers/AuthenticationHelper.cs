using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Xamarin.Auth;
using Xamarin.SNS.Login.Sample.Models;
using Xamarin.SNS.Login.Sample.Models.Providers;

namespace Xamarin.SNS.Login.Sample.Helpers
{
    public static class AuthenticationHelper
    {
        public static async Task<User> GetUserInfo(OAuth2Base oAuth2, SNSProvider provider, Account account)
        {
            User user = null;
            string token = account.Properties["access_token"];
            string refreshToke = account.Properties["refresh_token"];
            int expriesIn;
            int.TryParse(account.Properties["expires_in"], out expriesIn);

            switch (provider)
            {
                case SNSProvider.Kakao:
                    {
                        var request = new OAuth2Request("GET", oAuth2.UserInfoUri, null, account);
                        var response = await request.GetResponseAsync();
                        if (response != null)
                        {
                            // Deserialize the data and store it in the account store
                            // The users email address will be used to identify data in SimpleDB

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
                                PictureUrl = kakaoUser.Properties.ProfileImage
                            };
                        }
                    }
                    break;
                case SNSProvider.Line:
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string> { { "Authorization", token } };
                        var request = new OAuth2Request("POST", oAuth2.UserInfoUri, null, account);
                        var response = await request.GetResponseAsync();
                        if (response != null)
                        {
                            // Deserialize the data and store it in the account store
                            // The users email address will be used to identify data in SimpleDB

                            string userJson = await response.GetResponseTextAsync();
                            var lineUser = JsonConvert.DeserializeObject<LineUser>(userJson);
                            user = new User
                            {
                                Id = lineUser.Id,
                                Token = token,
                                RefreshToken = refreshToke,
                                Name = lineUser.Name,
                                ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(expriesIn)),
                                PictureUrl = lineUser.ProfileImage
                            };
                        }
                    }
                    break;
            }

            return user;
        }

        public static async Task<bool> RefreshToken(SNSProvider provider)
        {
            bool refreshSuccess = false;
            var user = AppSettings.User;
            if (user == null)
            {
                return refreshSuccess;
            }
            var oAuth2 = OAuth2ProviderFactory.CreateProvider(provider);

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
            };

            switch(provider)
            {
                case SNSProvider.Kakao:
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string> { { "grant_type", "refresh_token" }, { "refresh_token", user.RefreshToken }, { "client_id", user.Id } };
                        var request = new OAuth2Request("POST", oAuth2.RequestTokenUri, dictionary, null);
                        var response = await request.GetResponseAsync();
                        if (response != null)
                        {
                            // Deserialize the data and store it in the account store
                            // The users email address will be used to identify data in SimpleDB

                            string tokenString = await response.GetResponseTextAsync();
                            JObject jwtDynamic = JsonConvert.DeserializeObject<JObject>(tokenString, serializerSettings);
                            var accessToken = jwtDynamic.Value<string>("access_token");
                            var refreshToken = jwtDynamic.Value<string>("refresh_token");
                            var expiresIn = jwtDynamic.Value<int>("expires_in");


                            user.Token = accessToken;
                            user.RefreshToken = refreshToken;
                            user.ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(expiresIn));

                            refreshSuccess = true;
                        }
                    }
                    break;
                case SNSProvider.Line:
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string> { { "grant_type", "refresh_token" }, {"refresh_token", user.RefreshToken }, {"client_id", user.Id} };
                        var request = new OAuth2Request("POST", oAuth2.RequestTokenUri, dictionary, null);
                        var response = await request.GetResponseAsync();
                        if (response != null)
                        {
                            // Deserialize the data and store it in the account store
                            // The users email address will be used to identify data in SimpleDB

                            string tokenString = await response.GetResponseTextAsync();
                            JObject jwtDynamic = JsonConvert.DeserializeObject<JObject>(tokenString, serializerSettings);
                            var accessToken = jwtDynamic.Value<string>("access_token");
                            var refreshToken = jwtDynamic.Value<string>("refresh_token");
                            var expiresIn = jwtDynamic.Value<int>("expires_in");


                            user.Token = accessToken;
                            user.RefreshToken = refreshToken;
                            user.ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(expiresIn));

                            refreshSuccess = true;
                        }
                    }
                    break;
            }

            AppSettings.User = user;

            return refreshSuccess;
        }
    }
}
