using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;
using Xamarin.SNS.Login.Sample.Models;
using Xamarin.SNS.Login.Sample.Services.Authentication;

namespace Xamarin.SNS.Login.Sample.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        #region private member area
        private IPageDialogService dialogService;
        private IAuthenticationService authenticationService;
        private DelegateCommand<SNSProvider?> snsSignInCommand;
        //private DelegateCommand snsSignInCommand;
        #endregion

        public LoginPageViewModel(INavigationService navigationService, IPageDialogService dialogService, IAuthenticationService authenticationService)
            : base(navigationService)
        {
            Title = "Login Page";
            this.dialogService = dialogService;
            this.authenticationService = authenticationService;
            MessagingInit();
        }

        #region Command Area
        public DelegateCommand<SNSProvider?> SNSSignInCommand =>
                                    snsSignInCommand ?? (snsSignInCommand =
                                                         new DelegateCommand<SNSProvider?>
                                                             (
                                                                async (SNSProvider? snsProvider) =>
                                                                {
                                                                    IsBusy = true;
                                                                    try
                                                                    {
                                                                        //var provider = snsProvider ?? SNSProvider.None;
                                                                        if (snsProvider.HasValue)
                                                                        {
                                                                            await authenticationService.LoginWithSNSAsync(snsProvider.Value);
                                                                        }
                                                                    }
                                                                    catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
                                                                    {
                                                                        await dialogService.DisplayAlertAsync("네트웍 에러 입니다.", "에러", "Ok");
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        Debug.WriteLine($"Error in: {ex}");
                                                                    }
                                                                    finally
                                                                    {
                                                                        IsBusy = false;
                                                                    }
                                                                }
                                                            )
                                                        );
        #endregion

        #region Messaging Setting Area
        void MessagingInit()
        {
            MessagingCenter.Subscribe<User, bool>(this, MessengerKeys.AuthenticationRequested, OnAuthenticationChanged);
        }

        private async void OnAuthenticationChanged(User user, bool isLogin)
        {
            if (isLogin)
            {
                var p = new NavigationParameters();
                p.Add("user", user);

                await NavigationService.NavigateAsync("NavigationPage/MainPage", p);
            }
        }
        #endregion
    }
}
