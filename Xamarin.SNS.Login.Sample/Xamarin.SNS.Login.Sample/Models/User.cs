using System;
namespace Xamarin.SNS.Login.Sample.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public DateTime ExpiresIn { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PictureUrl { get; set; }

        public bool LoggedInWithSNSAccount { get; set; }

        public SNSProvider Provider { get; set; }
    }
}
