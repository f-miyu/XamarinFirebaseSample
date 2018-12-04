using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Prism.Logging;
using Prism.Services;
using XamarinFirebaseSample.Services;
using System.Reactive.Disposables;
using Reactive.Bindings;
using Prism.NavigationEx;
using System.Threading;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;

namespace XamarinFirebaseSample.ViewModels
{
    public class EmailLoginPageViewModel : ViewModelBase
    {
        private readonly IEmailLoginService _emailLoginService;
        private readonly IPageDialogService _pageDialogService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ReactivePropertySlim<string> Email { get; }
        public ReactivePropertySlim<string> Password { get; }

        public AsyncReactiveCommand LoginCommand { get; }

        protected EmailLoginPageViewModel(INavigationService navigationService, IEmailLoginService emailLoginService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _emailLoginService = emailLoginService;
            _pageDialogService = pageDialogService;

            Title = "ログイン";

            Email = _emailLoginService.Email;
            Password = _emailLoginService.Password;

            _emailLoginService.LoginCompletedNotifier
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

            _emailLoginService.LoginErrorNotifier
                              .ObserveOn(SynchronizationContext.Current)
                              .Subscribe(_ => _pageDialogService.DisplayAlertAsync("エラー", "ログインに失敗しました", "OK"))
                              .AddTo(_disposables);

            _emailLoginService.IsLoggingIn
                              .Skip(1)
                              .Where(b => b)
                              .ObserveOn(SynchronizationContext.Current)
                              .SelectMany(_ => NavigateAsync<LoadingPageViewModel>())
                              .Subscribe()
                              .AddTo(_disposables);

            _emailLoginService.IsLoggingIn
                              .Skip(1)
                              .Where(b => !b)
                              .ObserveOn(SynchronizationContext.Current)
                              .SelectMany(_ => GoBackAsync())
                              .Subscribe()
                              .AddTo(_disposables);

            LoginCommand = _emailLoginService.CanLogin
                                             .ToAsyncReactiveCommand()
                                             .AddTo(_disposables);

            LoginCommand.Subscribe(async () => await _emailLoginService.Login());
        }

        public override void Destroy()
        {
            base.Destroy();

            _disposables.Dispose();
        }
    }
}
