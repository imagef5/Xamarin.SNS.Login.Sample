using System;
using Newtonsoft.Json;

namespace Xamarin.SNS.Login.Sample.Models.Providers
{
    [JsonObject]
    public class KakaoUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("kaccount_email")]
        public string Email { get; set; }

        [JsonProperty("kaccount_email_verified")]
        public bool VerifiedEmail { get; set; }

        public Properties Properties { get; set; }
    }

    [JsonObject]
    public class Properties
    {
        [JsonProperty("nickname")]
        public string NickName { get; set; }

        [JsonProperty("thumbnail_image")]
        public string Thumbnail { get; set; }

        [JsonProperty("profile_image")]
        public string ProfileImage { get; set; }
    }
}
