using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Prism.Logging;
using Prism.Services;
using Plugin.FirebaseAuth;

namespace XamarinFirebaseSample.ViewModels
{
    public class SplashScreenPageViewModel : ViewModelBase
    {
        private IListenerRegistration _registration;

        public SplashScreenPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _registration = CrossFirebaseAuth.Current.Instance.AddAuthStateChangedListener(user =>
            {
                NavigateAsync<MainPageViewModel>(wrapInNavigationPage: true, noHistory: true);
            });
        }

        public override void Destroy()
        {
            base.Destroy();

            _registration.Remove();
        }
    }
}
