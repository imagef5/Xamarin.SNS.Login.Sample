using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.SNS.Login.Sample.Models;
using Xamarin.SNS.Login.Sample.Models.Providers;

namespace Xamarin.SNS.Login.Sample.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
	{
		OAuth2Base oAuth2;

		public bool IsAuthenticated => AppSettings.User != null;

		public User AuthenticatedUser => AppSettings.User;

		public Task<bool> LoginAsync(string email, string password)
		{
			var user = new User
			{
				Email = email,
				Name = email,
				LastName = string.Empty,
				PictureUrl = "",
				Token = email,
				LoggedInWithSNSAccount = false,
				Provider = SNSProvider.None
			};

			AppSettings.User = user;

			return Task.FromResult(true);
		}
        
		public Task LoginWithSNSAsync(SNSProvider provider)
		{
			try
			{
				oAuth2 = OAuth2ProviderFactory.CreateProvider(provider);
				var authenticator = new OAuth2Authenticator(
					oAuth2.ClientId,
					oAuth2.ClientSecret,
					oAuth2.Scope,
					oAuth2.AuthorizationUri,
					oAuth2.RedirectUri,
					oAuth2.RequestTokenUri,
					null,
					false);

			//authenticator.Completed += OnAuthCompleted;
			//authenticator.Error += OnAuthError;
				authenticator.Completed += async (s, e) =>
				{
					//var auth2Authenticator = s as OAuth2Authenticator;
					if (e.IsAuthenticated)
					{
						// If the user is authenticated, request their basic user data from Google
						// UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
						var user = await oAuth2.GetUserInfoAsync(e.Account);

						AppSettings.User = user;
                        MessagingCenter.Send(user, MessengerKeys.AuthenticationRequested, true);
						Debug.WriteLine("Authentication Success");
					}
				};
				authenticator.Error += (s, e) =>
				{
					Debug.WriteLine("Authentication error: " + e.Message);
				};

				var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
				presenter.Login(authenticator);

			}
			catch (Exception ex)
			{
				Debug.WriteLine("Login Error : " + ex.Message);
                return Task.FromResult(false);
			}
			return Task.FromResult(true);
		}

		public async Task<bool> UserIsAuthenticatedAndValidAsync()
		{
			if (!IsAuthenticated)
			{
				return false;
			}
			else if (!AuthenticatedUser.LoggedInWithSNSAccount)
			{
				return true;
			}
			else
			{
				bool refreshSucceded = false;
				oAuth2 = OAuth2ProviderFactory.CreateProvider(AuthenticatedUser.Provider);
				try
				{
					var utcNow = DateTime.UtcNow.AddMinutes(30);
					if (AuthenticatedUser.ExpiresIn < utcNow)
					{
						var ret = await oAuth2.RefreshTokenAsync(AuthenticatedUser);
						if (ret.IsRefresh)
						{
							AppSettings.User = ret.User;
						}
                        else
                        {
                            AppSettings.RemoveUserData();
                        }

                        refreshSucceded = ret.IsRefresh;
					}
                    else
                    {
                        refreshSucceded = true;
                    }
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"Error with refresh attempt: {ex}");
				}

				return refreshSucceded;
			}
		}

		public Task LogoutAsync()
		{
			AppSettings.RemoveUserData();
			return Task.FromResult(true);
		}
	}
}
