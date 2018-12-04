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
using Reactive.Bindings.Extensions;
using System.Threading;
using System.Reactive.Linq;
using Prism.NavigationEx;

namespace XamarinFirebaseSample.ViewModels
{
    public class SignupPageViewModel : ViewModelBase
    {
        private readonly ISignupService _signupService;
        private readonly IPageDialogService _pageDialogService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ReactivePropertySlim<string> Email { get; }
        public ReactivePropertySlim<string> Name { get; }
        public ReactivePropertySlim<string> Password { get; }

        public AsyncReactiveCommand SignupCommand { get; }

        public SignupPageViewModel(INavigationService navigationService, ISignupService signupService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _signupService = signupService;
            _pageDialogService = pageDialogService;

            Title = "サインアップ";

            Email = _signupService.Email;
            Name = _signupService.Name;
            Password = _signupService.Password;

            _signupService.Image.Value = "user.png";

            _signupService.SignupCompletedNotifier
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

            _signupService.SignupErrorNotifier
                               .ObserveOn(SynchronizationContext.Current)
                               .Subscribe(_ => _pageDialogService.DisplayAlertAsync("エラー", "サインインに失敗しました", "OK"))
                               .AddTo(_disposables);

            _signupService.IsSigningUp
                          .Skip(1)
                          .Where(b => b)
                          .ObserveOn(SynchronizationContext.Current)
                          .SelectMany(_ => NavigateAsync<LoadingPageViewModel>())
                          .Subscribe()
                          .AddTo(_disposables);

            _signupService.IsSigningUp
                          .Skip(1)
                          .Where(b => !b)
                          .ObserveOn(SynchronizationContext.Current)
                          .SelectMany(_ => GoBackAsync())
                          .Subscribe()
                          .AddTo(_disposables);

            SignupCommand = _signupService.CanSignup
                                          .ToAsyncReactiveCommand()
                                          .AddTo(_disposables);

            SignupCommand.Subscribe(async () => await _signupService.Signup());
        }

        public override void Destroy()
        {
            base.Destroy();

            _disposables.Dispose();
        }
    }
}
