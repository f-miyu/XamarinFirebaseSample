using System;
using Reactive.Bindings;
using System.Reactive.Linq;
using Plugin.CloudFirestore;
using XamarinFirebaseSample.Models;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using Nito.AsyncEx;

namespace XamarinFirebaseSample.Services
{
    public class ItemService : IItemService
    {
        private readonly IAccountService _accountService;
        private readonly IFirestore _firestore;
        private readonly AsyncLock _lock = new AsyncLock();

        private readonly ReactivePropertySlim<Item> _item = new ReactivePropertySlim<Item>();
        public ReadOnlyReactivePropertySlim<Item> Item { get; }

        private readonly ReactivePropertySlim<User> _owner = new ReactivePropertySlim<User>();
        public ReadOnlyReactivePropertySlim<User> Owner { get; }

        private readonly ReactivePropertySlim<bool> _isLiked = new ReactivePropertySlim<bool>();
        public ReadOnlyReactivePropertySlim<bool> IsLiked { get; }

        public ReadOnlyReactivePropertySlim<bool> IsOwner { get; }

        private readonly Subject<string> _loadErrorNotifier = new Subject<string>();
        public IObservable<string> LoadErrorNotifier => _loadErrorNotifier;

        public ItemService(IAccountService accountService)
        {
            _accountService = accountService;
            _firestore = CrossCloudFirestore.Current.Instance;

            Item = _item.ToReadOnlyReactivePropertySlim();
            Owner = _owner.ToReadOnlyReactivePropertySlim();
            IsLiked = _isLiked.ToReadOnlyReactivePropertySlim();
            IsOwner = Observable.CombineLatest(Item, _accountService.UserId, (item, userId) => item != null && item.Id == userId)
                                .ToReadOnlyReactivePropertySlim();
        }

        public async Task LoadAsync(string id)
        {
            try
            {
                var itemDocument = await _firestore.GetCollection(Models.Item.CollectionPath)
                                                   .GetDocument(id)
                                                   .GetDocumentAsync()
                                                   .ConfigureAwait(false);

                var item = itemDocument.ToObject<Item>();

                if (item != null)
                {
                    _item.Value = item;

                    var likeTask = _firestore.GetDocument($"{User.CollectionPath}/{_accountService.UserId}/{Like.CollectionPath}/{id}")
                                             .GetDocumentAsync();

                    if (!string.IsNullOrEmpty(item.OwnerId))
                    {
                        var ownerDocument = await _firestore.GetCollection(User.CollectionPath)
                                                            .GetDocument(item.OwnerId)
                                                            .GetDocumentAsync()
                                                            .ConfigureAwait(false);

                        _owner.Value = ownerDocument.ToObject<User>();
                    }

                    var likeDocument = await likeTask.ConfigureAwait(false);

                    _isLiked.Value = likeDocument.Exists;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                _loadErrorNotifier.OnNext(e.Message);
            }
        }

        public async Task LikeOrUnlikeAsync()
        {
            if (!IsLiked.Value)
            {
                await LikeAsync();
            }
            else
            {
                await UnlikeAsync();
            }
        }

        public async Task LikeAsync()
        {
            if (Item.Value == null || IsLiked.Value)
                return;

            _isLiked.Value = true;
            Item.Value.LikeCount++;

            using (await _lock.LockAsync())
            {
                try
                {
                    var success = await _firestore.RunTransactionAsync(transaction =>
                    {
                        var document = _firestore.GetCollection(Models.Item.CollectionPath)
                                                 .GetDocument(Item.Value.Id);

                        var item = transaction.GetDocument(document).ToObject<Item>();

                        if (item == null)
                            return false;

                        item.LikeCount++;
                        transaction.UpdateData(document, item);

                        return true;
                    }).ConfigureAwait(false);

                    if (success)
                    {
                        var like = new Like
                        {
                            IsLiked = true,
                            Timestamp = DateTime.Now.Ticks
                        };

                        await _firestore.GetDocument($"{User.CollectionPath}/{_accountService.UserId.Value}/{Like.CollectionPath}/{Item.Value.Id}")
                                        .SetDataAsync(like)
                                        .ConfigureAwait(false);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
        }

        public async Task UnlikeAsync()
        {
            if (Item.Value == null || !IsLiked.Value)
                return;

            _isLiked.Value = false;
            Item.Value.LikeCount--;

            using (await _lock.LockAsync())
            {
                try
                {
                    var success = await _firestore.RunTransactionAsync(transaction =>
                    {
                        var document = _firestore.GetCollection(Models.Item.CollectionPath)
                                                 .GetDocument(Item.Value.Id);

                        var item = transaction.GetDocument(document).ToObject<Item>();

                        if (item == null)
                            return false;

                        item.LikeCount--;
                        transaction.UpdateData(document, item);

                        return true;
                    }).ConfigureAwait(false);

                    if (success)
                    {
                        await _firestore.GetDocument($"{User.CollectionPath}/{_accountService.UserId.Value}/{Models.Like.CollectionPath}/{Item.Value.Id}")
                                        .DeleteDocumentAsync()
                                        .ConfigureAwait(false);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
        }
    }
}
