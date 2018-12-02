using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Prism.Logging;
using Prism.Services;
using XamarinFirebaseSample.Services;
using Reactive.Bindings;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;
using Prism.NavigationEx;
using System.Threading;

namespace XamarinFirebaseSample.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly IGoogleLoginService _googleLoginService;
        private readonly IPageDialogService _pageDialogService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly ReactivePropertySlim<bool> _isIdle = new ReactivePropertySlim<bool>(true);

        public AsyncReactiveCommand LoginWithGoogleCommand { get; }
        public AsyncReactiveCommand LoginWithEmailCommand { get; }
        public AsyncReactiveCommand SignupCommand { get; }

        public LoginPageViewModel(INavigationService navigationService, IGoogleLoginService googleLoginService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _googleLoginService = googleLoginService;
            _pageDialogService = pageDialogService;

            _googleLoginService.LoginCompletedNotifier
                               .ObserveOn(SynchronizationContext.Current)
                               .SelectMany(_ =>
                               {
                                   var navigation = NavigationFactory.Create<MainPageViewModel>()
                                                                     .Add(NavigationNameProvider.DefaultNavigationPageName)
                                                                     .Add<HomePageViewModel>();
                                   return NavigateAsync(navigation, noHistory: true);
                               })
                               .Subscribe()
                               .AddTo(_disposables);

            _googleLoginService.LoginErrorNotifier
                               .ObserveOn(SynchronizationContext.Current)
                               .Subscribe(_ => _pageDialogService.DisplayAlertAsync("エラー", "ログインに失敗しました", "OK"))
                               .AddTo(_disposables);

            _googleLoginService.DoingLoginNotifier
                               .Skip(1)
                               .Where(b => b)
                               .ObserveOn(SynchronizationContext.Current)
                               .SelectMany(_ => NavigateAsync<LoadingPageViewModel>())
                               .Subscribe()
                               .AddTo(_disposables);

            _googleLoginService.DoingLoginNotifier
                               .Skip(1)
                               .Where(b => !b)
                               .ObserveOn(SynchronizationContext.Current)
                               .SelectMany(_ => GoBackAsync())
                               .Subscribe()
                               .AddTo(_disposables);

            LoginWithGoogleCommand = _isIdle.ToAsyncReactiveCommand();
            LoginWithGoogleCommand.Subscribe(async () => await _googleLoginService.Login());

            LoginWithEmailCommand = _isIdle.ToAsyncReactiveCommand();
            LoginWithEmailCommand.Subscribe(async () => await NavigateAsync<EmailLoginPageViewModel>());

            SignupCommand = _isIdle.ToAsyncReactiveCommand();
            SignupCommand.Subscribe(async () => await NavigateAsync<SignupPageViewModel>());
        }

        public override void Destroy()
        {
            base.Destroy();

            _disposables.Dispose();
        }
    }
}
