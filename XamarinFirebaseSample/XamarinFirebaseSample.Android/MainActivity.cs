using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Plugin.CurrentActivity;
using Prism;
using Prism.Events;
using Prism.Ioc;
using Rg.Plugins.Popup;
using Rg.Plugins.Popup.Services;
using Xamarin.Auth;
using XamarinFirebaseSample.Views;
using Android.Service.Autofill;

namespace XamarinFirebaseSample.Droid
{
    [Activity(Label = "@string/app_name",
              Icon = "@mipmap/ic_launcher",
              Theme = "@style/MainTheme",
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity Instance { get; private set; }

        public App App { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Instance = this;

            CrossCurrentActivity.Current.Init(this, bundle);

            Plugin.CloudFirestore.CloudFirestore.Init(this);
            Plugin.FirebaseAuth.FirebaseAuth.Init(this);
            Plugin.FirebaseStorage.FirebaseStorage.Init(this);
            Plugin.FirebaseAnalytics.FirebaseAnalytics.Init(this);
            Fabric.Fabric.With(this, new Crashlytics.Crashlytics());

            Crashlytics.Crashlytics.HandleManagedExceptions();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, bundle);
            Rg.Plugins.Popup.Popup.Init(this, bundle);

            CustomTabsConfiguration.CustomTabsClosingMessage = null;

            global::Xamarin.Forms.Forms.Init(this, bundle);

            App = new App(new AndroidInitializer());
            LoadApplication(App);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            var page = PopupNavigation.Instance.PopupStack.LastOrDefault();
            if (!(page is LoadingPage))
            {
                Popup.SendBackPressed(base.OnBackPressed);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            App.Destroy();
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }
}

