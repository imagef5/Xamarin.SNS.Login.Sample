using System;
using System.Threading.Tasks;
using Xamarin.SNS.Login.Sample.Models;

namespace Xamarin.SNS.Login.Sample.Services.Authentication
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated { get; }

        User AuthenticatedUser { get; }
        
        Task<bool> LoginAsync(string email, string password);

        Task LoginWithSNSAsync(SNSProvider provider);

        Task<bool> UserIsAuthenticatedAndValidAsync();

        Task LogoutAsync();
    }
}
