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
using Prism.NavigationEx;
using DryIoc;
using System.Threading;

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
                           .ObserveOn(SynchronizationContext.Current)
                           .SelectMany(_ =>
                           {
                               if (_accountService.IsLoggedIn.Value)
                               {
                                   var navigation = NavigationFactory.Create<MainPageViewModel>()
                                                                     .Add(NavigationNameProvider.DefaultNavigationPageName)
                                                                     .Add<HomePageViewModel>();
                                   return NavigateAsync(navigation, noHistory: true);
                               }
                               return NavigateAsync<LoginPageViewModel>(wrapInNavigationPage: true, noHistory: true);
                           })
                           .Subscribe()
                           .AddTo(_disposables);

            _accountService.Initialize();
        }

        public override void Destroy()
        {
            base.Destroy();

            _disposables.Dispose();
        }
    }
}
