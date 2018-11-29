using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Prism.Logging;
using Prism.Services;
using Reactive.Bindings;
using XamarinFirebaseSample.Services;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;

namespace XamarinFirebaseSample.ViewModels
{
    public class ItemDetailPageViewModel : ViewModelBase<string>
    {
        private readonly IItemService _itemService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ReadOnlyReactivePropertySlim<string> ItemImage { get; }
        public ReadOnlyReactivePropertySlim<string> ItemTitle { get; }
        public ReadOnlyReactivePropertySlim<int> LikeCount { get; }
        public ReadOnlyReactivePropertySlim<string> OwnerName { get; }
        public ReadOnlyReactivePropertySlim<string> OwnerImage { get; }
        public ReadOnlyReactivePropertySlim<bool> IsLiked { get; }
        public ReadOnlyReactivePropertySlim<bool> IsOwner { get; }

        public ReactiveCommand LikeOrUnlikeCommand { get; } = new ReactiveCommand();

        public ItemDetailPageViewModel(INavigationService navigationService, IItemService itemService) : base(navigationService)
        {
            _itemService = itemService;

            ItemImage = _itemService.Item
                                    .Select(item => item != null ? item.ObserveProperty(i => i.Image) : Observable.Return<string>(null))
                                    .Switch()
                                    .ToReadOnlyReactivePropertySlim()
                                    .AddTo(_disposables);

            ItemTitle = _itemService.Item
                                    .Select(item => item != null ? item.ObserveProperty(i => i.Title) : Observable.Return<string>(null))
                                    .Switch()
                                    .ToReadOnlyReactivePropertySlim()
                                    .AddTo(_disposables);

            LikeCount = _itemService.Item
                                    .Select(item => item != null ? item.ObserveProperty(i => i.LikeCount) : Observable.Return(0))
                                    .Switch()
                                    .ToReadOnlyReactivePropertySlim()
                                    .AddTo(_disposables);

            OwnerName = _itemService.Owner
                                    .Select(user => user != null ? user.ObserveProperty(u => u.Name) : Observable.Return<string>(null))
                                    .Switch()
                                    .ToReadOnlyReactivePropertySlim()
                                    .AddTo(_disposables);

            OwnerImage = _itemService.Owner
                                     .Select(user => user != null ? user.ObserveProperty(u => u.Image) : Observable.Return<string>(null))
                                     .Switch()
                                     .ToReadOnlyReactivePropertySlim()
                                     .AddTo(_disposables);

            IsLiked = _itemService.IsLiked;

            IsOwner = _itemService.IsOwner;

            LikeOrUnlikeCommand.Subscribe(() => _itemService.LikeOrUnlikeAsync());
        }

        public override void Prepare(string parameter)
        {
            _itemService.LoadAsync(parameter);
        }

        public override void Destroy()
        {
            base.Destroy();

            _disposables.Dispose();
        }
    }
}
