using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Prism.Logging;
using Prism.Services;
using Plugin.FirebaseAuth;
using XamarinFirebaseSample.Services;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;

namespace XamarinFirebaseSample.ViewModels
{
    public class SplashScreenPageViewModel : ViewModelBase
    {
        private readonly IAccountService _accountService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public SplashScreenPageViewModel(INavigationService navigationService, IAccountService accountService) : base(navigationService)
        {
            _accountService = accountService;

            _accountService.IsInitialized
                           .Where(b => b)
                           .Take(1)
                           .Subscribe(_ =>
                           {
                               if (_accountService.IsLoggedIn.Value)
                               {
                                   NavigateAsync<HomePageViewModel>(wrapInNavigationPage: true, noHistory: true);
                               }
                               else
                               {
                                   NavigateAsync<LoginPageViewModel>(wrapInNavigationPage: true, noHistory: true);
                               }
                           })
                           .AddTo(_disposables);
        }

        public override void Destroy()
        {
            base.Destroy();

            _disposables.Dispose();
        }
    }
}
