using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Prism.Logging;
using Prism.Services;
using XamarinFirebaseSample.Services;
using Prism.Events;
using Reactive.Bindings;
using XamarinFirebaseSample.Events;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;

namespace XamarinFirebaseSample.ViewModels
{
    public class MyPageViewModel : ViewModelBase
    {
        private readonly IUserItemListService _userItemListService;
        private readonly IAccountService _accountService;
        private readonly IEventAggregator _eventAggregator;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ReadOnlyReactiveCollection<ItemViewModel> Items { get; }
        public ReadOnlyReactivePropertySlim<string> UserName { get; }
        public ReadOnlyReactivePropertySlim<string> UserImage { get; }
        public ReadOnlyReactivePropertySlim<int> ContributionCount { get; }

        public AsyncReactiveCommand<ItemViewModel> LoadMoreCommand { get; } = new AsyncReactiveCommand<ItemViewModel>();
        public AsyncReactiveCommand<ItemViewModel> GoToItemDetailPageCommand { get; } = new AsyncReactiveCommand<ItemViewModel>();

        public MyPageViewModel(INavigationService navigationService, IUserItemListService userItemListService, IAccountService accountService, IEventAggregator eventAggregator) : base(navigationService)
        {
            _userItemListService = userItemListService;
            _accountService = accountService;
            _eventAggregator = eventAggregator;

            Title = "マイページ";

            _userItemListService.SetUserId(_accountService.UserId.Value);
            Items = _userItemListService.Items.ToReadOnlyReactiveCollection(i => new ItemViewModel(i)).AddTo(_disposables);

            UserName = _accountService.UserName;
            UserImage = _accountService.UserImage;
            ContributionCount = _accountService.ContributionCount;

            LoadMoreCommand.Subscribe(async item =>
            {
                if (item == Items?.LastOrDefault())
                {
                    await _userItemListService.LoadAsync();
                }
            });

            GoToItemDetailPageCommand.Subscribe(async viewModel => await NavigateAsync<ItemDetailPageViewModel, string>(viewModel.Id.Value));

            _eventAggregator.GetEvent<DestoryEvent>().Subscribe(_userItemListService.Close)
                            .AddTo(_disposables);

            _userItemListService.LoadAsync();
        }

        public override void Destroy()
        {
            base.Destroy();

            _userItemListService.Close();
            _disposables.Dispose();
        }
    }
}
