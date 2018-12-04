using System;
using System.Threading.Tasks;
using Plugin.CloudFirestore;
using Plugin.FirebaseStorage;
using Reactive.Bindings;
using System.IO;
using System.Reactive.Linq;
using System.Linq;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System.Reactive.Subjects;
using System.Reactive;
using XamarinFirebaseSample.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace XamarinFirebaseSample.Services
{
    public class ContributionService : IContributionService
    {
        private readonly IAccountService _accountService;
        private readonly IStorageService _storageService;
        private readonly IFirestore _firestore;

        public ReactivePropertySlim<Stream> ItemImage { get; } = new ReactivePropertySlim<Stream>();
        public ReactivePropertySlim<string> ItemTitle { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ItemComment { get; } = new ReactivePropertySlim<string>();
        public ReadOnlyReactivePropertySlim<bool> CanContribute { get; }

        private readonly BusyNotifier _contributingNotifier = new BusyNotifier();
        public ReadOnlyReactivePropertySlim<bool> IsContributing { get; }

        private readonly Subject<string> _contributeErrorNotifier = new Subject<string>();
        public IObservable<string> ContributeErrorNotifier => _contributeErrorNotifier;

        private readonly Subject<Unit> _contributeCompletedNotifier = new Subject<Unit>();
        public IObservable<Unit> ContributeCompletedNotifier => _contributeCompletedNotifier;

        public ContributionService(IAccountService accountService, IStorageService storageService)
        {
            _accountService = accountService;
            _storageService = storageService;
            _firestore = CrossCloudFirestore.Current.Instance;

            CanContribute = new[]
            {
                ItemImage.Select(s => s != null),
                ItemTitle.Select(s => !string.IsNullOrEmpty(s)),
                _accountService.UserId.Select(s => !string.IsNullOrEmpty(s))
            }
            .CombineLatestValuesAreAllTrue()
            .ToReadOnlyReactivePropertySlim();

            IsContributing = _contributingNotifier.ToReadOnlyReactivePropertySlim();
        }

        public async Task SelectImage()
        {
            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            });

            if (file != null)
            {
                ItemImage.Value?.Dispose();
                ItemImage.Value = file.GetStream();
                file.Dispose();
            }
        }

        public async Task Contribute()
        {
            if (!CanContribute.Value)
                return;

            try
            {
                using (_contributingNotifier.ProcessStart())
                {
                    var id = Guid.NewGuid().ToString();

                    var uri = await _storageService.UploadImage(ItemImage.Value, $"items/{id}/image.jpg").ConfigureAwait(false);

                    var item = new Item
                    {
                        Title = ItemTitle.Value,
                        Image = uri.AbsoluteUri,
                        Comment = ItemComment.Value,
                        OwnerId = _accountService.UserId.Value,
                        Timestamp = DateTime.Now.Ticks
                    };

                    await _firestore.GetCollection(Item.CollectionPath)
                        .GetDocument(id)
                        .SetDataAsync(item)
                        .ConfigureAwait(false);

                    await _accountService.IncrementContributionCountAsync(1);
                }
                _contributeCompletedNotifier.OnNext(Unit.Default);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                _contributeErrorNotifier.OnNext(e.Message);
            }
        }
    }
}
