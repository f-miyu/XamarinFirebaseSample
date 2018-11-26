using System;
using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using XamarinFirebaseSample.Models;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Linq;
using Plugin.CloudFirestore.Extensions;
using Reactive.Bindings.Extensions;

namespace XamarinFirebaseSample.Services
{
    public class ItemListService : IItemListService
    {
        private const int Count = 20;

        private readonly IFirestore _firestore;
        private readonly ObservableCollection<Item> _items = new ObservableCollection<Item>();
        private CompositeDisposable _disposables;

        private long _lastTimestamp = long.MaxValue;

        public ReadOnlyObservableCollection<Item> Items { get; }

        public ItemListService()
        {
            _firestore = CrossCloudFirestore.Current.Instance;

            Items = new ReadOnlyObservableCollection<Item>(_items);
        }

        public async Task LoadAsync()
        {
            try
            {
                var documents = await _firestore.GetCollection(Item.CollectionPath)
                                                .OrderBy(nameof(Item.Timestamp), true)
                                                .LimitTo(Count)
                                                .StartAfter(new long[] { _lastTimestamp })
                                                .GetDocumentsAsync()
                                                .ConfigureAwait(false);

                if (!documents.IsEmpty || _disposables == null)
                {
                    if (!documents.IsEmpty)
                    {
                        var lastItem = documents.Documents.Last().ToObject<Item>();
                        _lastTimestamp = lastItem.Timestamp;
                    }
                    else
                    {
                        _lastTimestamp = DateTime.Now.Ticks;
                    }

                    _disposables?.Dispose();
                    _disposables = new CompositeDisposable();

                    var observeQuery = _firestore.GetCollection(Item.CollectionPath)
                                                     .OrderBy(nameof(Item.Timestamp), true)
                                                     .EndAt(new long[] { _lastTimestamp });

                    observeQuery.ObserveAdded()
                                .Where(d => _items.FirstOrDefault(i => i.Id == d.Document.Id) == null)
                                .Subscribe(d => _items.Insert(d.NewIndex, d.Document.ToObject<Item>()))
                                .AddTo(_disposables);

                    observeQuery.ObserveModified()
                                .Select(d => d.Document.ToObject<Item>())
                                .Subscribe(item =>
                                {
                                    var targetItem = _items.FirstOrDefault(i => i.Id == item.Id);
                                    if (targetItem != null)
                                    {
                                        item.CopyTo(targetItem);
                                    }
                                })
                                .AddTo(_disposables);

                    observeQuery.ObserveRemoved()
                                .Select(d => _items.FirstOrDefault(i => i.Id == d.Document.Id))
                                .Where(item => item != null)
                                .Subscribe(item => _items.Remove(item))
                                .AddTo(_disposables);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public Task AddItemAsync(string title, string image)
        {
            var item = new Item
            {
                Title = title,
                Image = image,
                Timestamp = DateTime.Now.Ticks
            };

            return _firestore.GetCollection(Item.CollectionPath)
                             .AddDocumentAsync(item);
        }

        public void Close()
        {
            _disposables.Dispose();
        }
    }
}
