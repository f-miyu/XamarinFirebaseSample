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

namespace XamarinFirebaseSample.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private readonly IItemListService _itemListService;
        private readonly IEventAggregator _eventAggregator;

        public ReadOnlyReactiveCollection<ItemViewModel> Items { get; set; }

        public AsyncReactiveCommand<ItemViewModel> LoadMoreCommand { get; } = new AsyncReactiveCommand<ItemViewModel>();
        public AsyncReactiveCommand GoToContributionPageCommand { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand<ItemViewModel> GoToItemDetailPageCommand { get; } = new AsyncReactiveCommand<ItemViewModel>();

        public HomePageViewModel(INavigationService navigationService, IItemListService itemListService, IEventAggregator eventAggregator) : base(navigationService)
        {
            _itemListService = itemListService;
            _eventAggregator = eventAggregator;

            Title = "ホーム";

            Items = _itemListService.Items.ToReadOnlyReactiveCollection(i => new ItemViewModel(i));

            LoadMoreCommand.Subscribe(async item =>
            {
                if (item == Items?.LastOrDefault())
                {
                    await _itemListService.LoadAsync();
                }
            });

            GoToContributionPageCommand.Subscribe(async () => await NavigateAsync<ContributionPageViewModel>());
            GoToItemDetailPageCommand.Subscribe(async viewModel => await NavigateAsync<ItemDetailPageViewModel, string>(viewModel.Id.Value));

            _eventAggregator.GetEvent<DestoryEvent>().Subscribe(_itemListService.Close);

            _itemListService.LoadAsync();
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);

            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                //_itemListService.LoadAsync();
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            _itemListService.Close();
        }
    }
}
