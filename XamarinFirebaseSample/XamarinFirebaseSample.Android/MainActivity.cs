using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Plugin.CurrentActivity;
using Prism;
using Prism.Ioc;

namespace XamarinFirebaseSample.Droid
{
    [Activity(Label = "XamarinFirebaseSample", Icon = "@mipmap/ic_launcher",
              Theme = "@style/SplashTheme", MainLauncher = true,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity Instance { get; private set; }

        public App App { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.Window.RequestFeature(WindowFeatures.ActionBar);
            base.SetTheme(Resource.Style.MainTheme);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Instance = this;

            CrossCurrentActivity.Current.Init(this, bundle);

            Plugin.CloudFirestore.CloudFirestore.Init(this);
            Plugin.FirebaseAuth.FirebaseAuth.Init(this);
            Plugin.FirebaseStorage.FirebaseStorage.Init(this);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            App = new App(new AndroidInitializer());
            LoadApplication(App);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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

