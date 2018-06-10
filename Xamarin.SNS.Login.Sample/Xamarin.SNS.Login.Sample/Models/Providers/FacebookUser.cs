using System;
using Newtonsoft.Json;

namespace Xamarin.SNS.Login.Sample.Models.Providers
{
    [JsonObject]
    public class FacebookUser
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public Picture Picture { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }

    public class Picture
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public bool IsSilhouette { get; set; }

        public string Url { get; set; }
    }
}
