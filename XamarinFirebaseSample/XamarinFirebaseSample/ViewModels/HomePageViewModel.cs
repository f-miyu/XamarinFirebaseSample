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
using Prism.Events;
using XamarinFirebaseSample.Events;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;

namespace XamarinFirebaseSample.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private readonly IItemListService _itemListService;
        private readonly IEventAggregator _eventAggregator;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ReadOnlyReactiveCollection<ItemViewModel> Items { get; }

        public AsyncReactiveCommand<ItemViewModel> LoadMoreCommand { get; } = new AsyncReactiveCommand<ItemViewModel>();
        public AsyncReactiveCommand GoToContributionPageCommand { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand<ItemViewModel> GoToItemDetailPageCommand { get; } = new AsyncReactiveCommand<ItemViewModel>();

        public HomePageViewModel(INavigationService navigationService, IItemListService itemListService, IEventAggregator eventAggregator) : base(navigationService)
        {
            _itemListService = itemListService;
            _eventAggregator = eventAggregator;

            Title = "ホーム";

            Items = _itemListService.Items.ToReadOnlyReactiveCollection(i => new ItemViewModel(i)).AddTo(_disposables);

            LoadMoreCommand.Subscribe(async item =>
            {
                if (item == Items?.LastOrDefault())
                {
                    await _itemListService.LoadAsync();
                }
            });

            GoToContributionPageCommand.Subscribe(async () => await NavigateAsync<ContributionPageViewModel>());
            GoToItemDetailPageCommand.Subscribe(async viewModel => await NavigateAsync<ItemDetailPageViewModel, string>(viewModel.Id.Value));

            _eventAggregator.GetEvent<DestoryEvent>().Subscribe(_itemListService.Close)
                            .AddTo(_disposables);

            _itemListService.LoadAsync();
        }

        public override void Destroy()
        {
            base.Destroy();

            _itemListService.Close();
            _disposables.Dispose();
        }
    }
}
