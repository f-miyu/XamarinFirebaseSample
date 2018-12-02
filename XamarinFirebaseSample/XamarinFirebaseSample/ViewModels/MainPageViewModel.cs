using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Reactive.Bindings;
using XamarinFirebaseSample.Services;
using Prism.Services;

namespace XamarinFirebaseSample.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private const string Home = "ホーム";
        private const string Logout = "ログアウト";
        private const string MyPage = "マイページ";

        private readonly IAccountService _accountService;
        private readonly IPageDialogService _pageDialogService;

        public ReadOnlyReactivePropertySlim<string> UserName { get; }
        public ReadOnlyReactivePropertySlim<string> UserImage { get; }

        public List<string> Menus { get; }

        public AsyncReactiveCommand<string> ShowDetailPageCommand { get; } = new AsyncReactiveCommand<string>();

        public MainPageViewModel(INavigationService navigationService, IAccountService accountService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _accountService = accountService;
            _pageDialogService = pageDialogService;

            UserName = _accountService.UserName;
            UserImage = _accountService.UserImage;

            Menus = new List<string>
            {
                Home,
                MyPage,
                Logout
            };

            ShowDetailPageCommand.Subscribe(async page =>
            {
                switch (page)
                {
                    case Home:
                        await NavigateAsync<HomePageViewModel>(wrapInNavigationPage: true);
                        break;
                    case MyPage:
                        await NavigateAsync<MyPageViewModel>(wrapInNavigationPage: true);
                        break;
                    case Logout:
                        var ok = await _pageDialogService.DisplayAlertAsync("確認", "ログアウトしてよろしいですか？", "OK", "キャンセル");
                        if (ok)
                        {
                            _accountService.Logout();
                            await NavigateAsync<LoginPageViewModel>(wrapInNavigationPage: true, noHistory: true);
                        }
                        break;
                }
            });
        }
    }
}
