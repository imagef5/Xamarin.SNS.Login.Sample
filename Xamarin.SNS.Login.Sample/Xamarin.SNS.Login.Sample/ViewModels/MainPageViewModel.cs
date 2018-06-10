using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.SNS.Login.Sample.Models;

namespace Xamarin.SNS.Login.Sample.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Private Fields
        private User user;
        #endregion


        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";
        }

        #region Property Area
        /// <summary>
        /// User 상세 정보
        /// </summary>
        public User User
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }
        #endregion

        #region override method area
        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);

            if (parameters.ContainsKey("user"))
                User = (User)parameters["user"];
        }
        #endregion
    }
}
