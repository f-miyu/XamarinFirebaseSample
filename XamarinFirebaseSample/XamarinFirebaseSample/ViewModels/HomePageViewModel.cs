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

namespace XamarinFirebaseSample.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private readonly IItemListService _itemListService;

        public ReadOnlyReactiveCollection<ItemViewModel> Items { get; }

        public AsyncReactiveCommand<ItemViewModel> LoadMoreCommand { get; } = new AsyncReactiveCommand<ItemViewModel>();
        public AsyncReactiveCommand AddItemCommand { get; } = new AsyncReactiveCommand();

        public HomePageViewModel(INavigationService navigationService, IItemListService itemListService) : base(navigationService)
        {
            _itemListService = itemListService;

            Title = "ホーム";

            Items = _itemListService.Items.ToReadOnlyReactiveCollection(i => new ItemViewModel(i));

            LoadMoreCommand.Subscribe(async item =>
            {
                if (item == Items.LastOrDefault())
                {
                    await _itemListService.LoadAsync();
                }
            });

            AddItemCommand.Subscribe(async () =>
            {
                await _itemListService.AddItemAsync("test", "");
            });
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);

            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                _itemListService.LoadAsync();
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            _itemListService.Close();
        }
    }
}
