using System;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.SNS.Login.Sample.Services.Authentication;
using Xamarin.SNS.Login.Sample.Views;

namespace Xamarin.SNS.Login.Sample.ViewModels
{
	public class SplashPageViewModel : ViewModelBase
	{
		#region private member area
		private IAuthenticationService authenticationService;
		private DelegateCommand authenticationCheckCommand;
		#endregion

		public SplashPageViewModel(INavigationService navigationService, IAuthenticationService authenticationService)
			: base(navigationService)
		{
			Title = "Splash Page";
			this.authenticationService = authenticationService;
		}

		#region Command Area
		public DelegateCommand AuthenticationCheckCommand =>
		authenticationCheckCommand ??( authenticationCheckCommand = 
		                              new DelegateCommand
										(
										  async () =>
											{
											await Task.Delay(3000);
    											if (await authenticationService.UserIsAuthenticatedAndValidAsync())
    											{
    												var user = AppSettings.User;
    												var p = new NavigationParameters();
    												p.Add("user", user);

    												await NavigationService.NavigateAsync("NavigationPage/MainPage", p);
    											}
    											else
    											{
                                                    await NavigationService.NavigateAsync("app:///LoginPage");
                                    			}
                                    		}
			                             )
                    		          );
		#endregion
	}
}