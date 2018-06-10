using Prism;
using Prism.Ioc;
using Xamarin.SNS.Login.Sample.ViewModels;
using Xamarin.SNS.Login.Sample.Views;
using Xamarin.SNS.Login.Sample.Services.Authentication;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Unity;
using Unity.Lifetime;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Xamarin.SNS.Login.Sample
{
    public partial class App : PrismApplication
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("SplashPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAuthenticationService, AuthenticationService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<LoginPage>();
            containerRegistry.RegisterForNavigation<SplashPage>();
        }
    }
}
