using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Droid;
using Lottie.Forms.Droid;
using Prism;
using Prism.Ioc;

namespace Xamarin.SNS.Login.Sample.Droid
{
    [Activity(Label = "Xamarin_SNS_Login_Sample", 
              Icon = "@mipmap/ic_launcher", 
              Theme = "@style/MainTheme", 
              MainLauncher = false,
              ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, bundle);
            //FFImageLoading 초기화
            CachedImageRenderer.Init(true);
            AnimationViewRenderer.Init();

            LoadApplication(new App(new AndroidInitializer()));
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {
            // Register any platform specific implementations
        }
    }
}

