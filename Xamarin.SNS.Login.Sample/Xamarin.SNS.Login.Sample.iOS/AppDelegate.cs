using FFImageLoading.Forms.Touch;
using Foundation;
using Lottie.Forms.iOS.Renderers;
using Prism;
using Prism.Ioc;
using UIKit;


namespace Xamarin.SNS.Login.Sample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Auth.Presenters.XamarinIOS.AuthenticationConfiguration.Init();

            //FFImageLoading 초기화
            CachedImageRenderer.Init();
            AnimationViewRenderer.Init();

            LoadApplication(new App(new iOSInitializer()));

            //UINavigationBar.Appearance.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            //UINavigationBar.Appearance.ShadowImage = new UIImage();
            //UINavigationBar.Appearance.BackgroundColor = UIColor.Clear;
            //UINavigationBar.Appearance.TintColor = UIColor.White;
            //UINavigationBar.Appearance.BarTintColor = UIColor.Clear;
            //UINavigationBar.Appearance.Translucent = true;

            return base.FinishedLaunching(app, options);
        }
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {

        }
    }
}
