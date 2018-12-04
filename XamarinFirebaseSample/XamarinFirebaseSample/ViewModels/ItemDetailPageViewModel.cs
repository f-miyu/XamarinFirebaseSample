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
using Xamarin.Forms;
using System.Threading;
using System.Reactive.Threading.Tasks;
using Plugin.Media;
using Plugin.FirebaseAnalytics;

namespace XamarinFirebaseSample.ViewModels
{
    public class ItemDetailPageViewModel : ViewModelBase<string>
    {
        private readonly IItemService _itemService;
        private readonly IPageDialogService _pageDialogService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ReadOnlyReactivePropertySlim<string> ItemImage { get; }
        public ReadOnlyReactivePropertySlim<string> ItemTitle { get; }
        public ReadOnlyReactivePropertySlim<string> ItemComment { get; }
        public ReadOnlyReactivePropertySlim<int> LikeCount { get; }
        public ReadOnlyReactivePropertySlim<string> OwnerName { get; }
        public ReadOnlyReactivePropertySlim<string> OwnerImage { get; }
        public ReadOnlyReactivePropertySlim<bool> IsLiked { get; }
        public ReadOnlyReactivePropertySlim<bool> IsOwner { get; }

        public ReactiveCommand LikeOrUnlikeCommand { get; }
        public AsyncReactiveCommand DeleteCommand { get; }

        public ItemDetailPageViewModel(INavigationService navigationService, IItemService itemService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _itemService = itemService;
            _pageDialogService = pageDialogService;

            Title = "アイテム";

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

            ItemComment = _itemService.Item
                                      .Select(item => item != null ? item.ObserveProperty(i => i.Comment) : Observable.Return<string>(null))
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

            _itemService.DeleteCompletedNotifier
                        .ObserveOn(SynchronizationContext.Current)
                        .SelectMany(_ => _pageDialogService.DisplayAlertAsync(null, "削除が完了しました", "OK").ToObservable())
                        .ObserveOn(SynchronizationContext.Current)
                        .SelectMany(_ => GoBackAsync())
                        .Subscribe()
                        .AddTo(_disposables);

            _itemService.DeleteErrorNotifier
                        .ObserveOn(SynchronizationContext.Current)
                        .SelectMany(_ => _pageDialogService.DisplayAlertAsync("エラー", "削除に失敗しました", "OK").ToObservable())
                        .Subscribe()
                        .AddTo(_disposables);

            _itemService.IsDeleting
                        .Skip(1)
                        .Where(b => b)
                        .ObserveOn(SynchronizationContext.Current)
                        .SelectMany(_ => NavigateAsync<LoadingPageViewModel>())
                        .Subscribe()
                        .AddTo(_disposables);

            _itemService.IsDeleting
                        .Skip(1)
                        .Where(b => !b)
                        .ObserveOn(SynchronizationContext.Current)
                        .SelectMany(_ => GoBackAsync())
                        .Subscribe()
                        .AddTo(_disposables);

            LikeOrUnlikeCommand = _itemService.IsLoaded
                                              .ObserveOn(SynchronizationContext.Current)
                                              .ToReactiveCommand()
                                              .AddTo(_disposables);

            LikeOrUnlikeCommand.Subscribe(() => _itemService.LikeOrUnlikeAsync());

            DeleteCommand = new[]
            {
                _itemService.IsLoaded,
                _itemService.IsOwner
            }.CombineLatestValuesAreAllTrue()
             .ObserveOn(SynchronizationContext.Current)
             .ToAsyncReactiveCommand()
             .AddTo(_disposables);

            DeleteCommand.Subscribe(async () =>
            {
                var ok = await _pageDialogService.DisplayAlertAsync("確認", "削除してよろしいですか？", "OK", "キャンセル");
                if (ok)
                {
                    await _itemService.DeleteAsync();
                }
            });
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
