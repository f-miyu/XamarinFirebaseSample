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

namespace XamarinFirebaseSample.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly IAccountService _accountService;
        private readonly IPageDialogService _pageDialogService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public AsyncReactiveCommand LoginWithGoogleCommand { get; } = new AsyncReactiveCommand();

        public LoginPageViewModel(INavigationService navigationService, IAccountService accountService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _accountService = accountService;
            _pageDialogService = pageDialogService;

            _accountService.IsLoggedIn
                           .Where(b => b)
                           .Take(1)
                           .Subscribe(_ => NavigateAsync<HomePageViewModel>(wrapInNavigationPage: true, noHistory: true))
                           .AddTo(_disposables);

            _accountService.LoginErrorNotifier
                           .Subscribe(_ => _pageDialogService.DisplayAlertAsync("エラー", "ログインに失敗しました", "OK"))
                           .AddTo(_disposables);

            LoginWithGoogleCommand.Subscribe(async () =>
            {
                await _accountService.LoginWithGoogle();
            });
        }

        public override void Destroy()
        {
            base.Destroy();

            _disposables.Dispose();
        }
    }
}
