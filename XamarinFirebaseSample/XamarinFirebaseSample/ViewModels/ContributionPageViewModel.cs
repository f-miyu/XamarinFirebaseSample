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
using Xamarin.Forms;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Reactive.Disposables;

namespace XamarinFirebaseSample.ViewModels
{
    public class ContributionPageViewModel : ViewModelBase
    {
        private readonly IContributionService _contributionService;
        private readonly IPageDialogService _pageDialogService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ReadOnlyReactivePropertySlim<ImageSource> ItemImage { get; }
        public ReactivePropertySlim<string> ItemTitle { get; }
        public ReactivePropertySlim<string> ItemComment { get; }
        public ReadOnlyReactivePropertySlim<bool> ItemImageNotExists { get; }

        public AsyncReactiveCommand SelectImageCommand { get; }
        public AsyncReactiveCommand ContributeCommand { get; }

        public ContributionPageViewModel(INavigationService navigationService, IContributionService contributionService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _contributionService = contributionService;
            _pageDialogService = pageDialogService;

            Title = "投稿";

            ItemImage = _contributionService.ItemImage
                                            .Select(source =>
                                            {
                                                if (source == null) return null;

                                                var stream = new MemoryStream();
                                                source.CopyTo(stream);
                                                source.Seek(0, SeekOrigin.Begin);
                                                stream.Seek(0, SeekOrigin.Begin);
                                                return stream;
                                            })
                                            .Select(s => s == null ? null : ImageSource.FromStream(() => s))
                                            .ToReadOnlyReactivePropertySlim()
                                            .AddTo(_disposables);

            ItemTitle = _contributionService.ItemTitle;
            ItemComment = _contributionService.ItemComment;
            ItemImageNotExists = _contributionService.ItemImage.Select(s => s == null)
                                                     .ToReadOnlyReactivePropertySlim()
                                                     .AddTo(_disposables);

            _contributionService.ContributeCompletedNotifier
                                .ObserveOn(SynchronizationContext.Current)
                                .SelectMany(_ => _pageDialogService.DisplayAlertAsync(null, "投稿が完了しました", "OK").ToObservable())
                                .ObserveOn(SynchronizationContext.Current)
                                .SelectMany(_ => GoBackAsync())
                                .Subscribe()
                                .AddTo(_disposables);

            _contributionService.ContributeErrorNotifier
                                .ObserveOn(SynchronizationContext.Current)
                                .SelectMany(_ => _pageDialogService.DisplayAlertAsync("エラー", "投稿に失敗しました", "OK").ToObservable())
                                .Subscribe()
                                .AddTo(_disposables);

            _contributionService.ContributingNotifier
                                .Skip(1)
                                .Where(b => b)
                                .ObserveOn(SynchronizationContext.Current)
                                .SelectMany(_ => NavigateAsync<LoadingPageViewModel>())
                                .Subscribe()
                                .AddTo(_disposables);

            _contributionService.ContributingNotifier
                                .Skip(1)
                                .Where(b => !b)
                                .ObserveOn(SynchronizationContext.Current)
                                .SelectMany(_ => GoBackAsync())
                                .Subscribe()
                                .AddTo(_disposables);

            SelectImageCommand = new AsyncReactiveCommand();
            ContributeCommand = _contributionService.CanContribute
                                                    .ToAsyncReactiveCommand()
                                                    .AddTo(_disposables);

            SelectImageCommand.Subscribe(async () => await _contributionService.SelectImage());
            ContributeCommand.Subscribe(async () => await _contributionService.Contribute());
        }

        public override void Destroy()
        {
            base.Destroy();

            _disposables.Dispose();
        }
    }
}
