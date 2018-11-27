
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XamarinFirebaseSample.Helpers;
using XamarinFirebaseSample.Services;
using Prism.Ioc;

namespace XamarinFirebaseSample.Droid
{
    [Activity(Label = "CustomUrlSchemeInterceptorActivity")]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = AppConstans.AndroidReversedGoogleClientId,
        DataPath = "/oauth2redirect")]
    public class CustomUrlSchemeInterceptorActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var authService = MainActivity.Instance.App.Container.Resolve<IAuthService>();
            authService.OnPageLoading(new Uri(Intent.Data.ToString()));

            var intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
            StartActivity(intent);

            Finish();
        }
    }
}
