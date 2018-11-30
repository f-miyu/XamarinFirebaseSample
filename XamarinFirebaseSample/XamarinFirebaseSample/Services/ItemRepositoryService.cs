using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Plugin.CloudFirestore;
using Reactive.Bindings.Notifiers;
using XamarinFirebaseSample.Models;

namespace XamarinFirebaseSample.Services
{
    public class ItemRepositoryService : IItemRepositoryService
    {
        private BusyNotifier _addingNotifier = new BusyNotifier();
        public IObservable<bool> AddingNotifier => _addingNotifier;

        private readonly Subject<string> _addErrorNotifier = new Subject<string>();
        public IObservable<string> AddErrorNotifier => _addErrorNotifier;

        private readonly Subject<Unit> _addCompletedNotifier = new Subject<Unit>();
        public IObserver<Unit> AddCompletedNotifier => _addCompletedNotifier;

        private BusyNotifier _updatingNotifier = new BusyNotifier();
        public IObservable<bool> UpdatingNotifier => _updatingNotifier;

        private readonly Subject<string> _updateErrorNotifier = new Subject<string>();
        public IObservable<string> UpdateErrorNotifier => _updateErrorNotifier;

        private readonly Subject<Unit> _updateCompletedNotifier = new Subject<Unit>();
        public IObserver<Unit> UpdateCompletedNotifier => _updateCompletedNotifier;

        private BusyNotifier _deletingNotifier = new BusyNotifier();
        public IObservable<bool> DeletingNotifier => _deletingNotifier;

        private readonly Subject<string> _deleteErrorNotifier = new Subject<string>();
        public IObservable<string> DeleteErrorNotifier => _deleteErrorNotifier;

        private readonly Subject<Unit> _deleteCompletedNotifier = new Subject<Unit>();
        public IObserver<Unit> DeleteCompletedNotifier => _deleteCompletedNotifier;

        public async Task AddAsync(string title, string image, string comment, string ownerId)
        {
            try
            {
                var item = new Item
                {
                    Title = title,
                    Image = image,
                    Comment = comment,
                    OwnerId = ownerId
                };

                using (_addingNotifier.ProcessStart())
                {
                    await CrossCloudFirestore.Current
                                             .Instance
                                             .GetCollection(Item.CollectionPath)
                                             .AddDocumentAsync(item)
                                             .ConfigureAwait(false);
                }

                _addCompletedNotifier.OnNext(Unit.Default);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                _addErrorNotifier.OnNext(e.Message);
            }
        }

        public async Task UpdateAsync(string itemId, string title, string image, string comment)
        {
            try
            {
                using (_updatingNotifier.ProcessStart())
                {
                    await CrossCloudFirestore.Current
                                             .Instance
                                             .GetCollection(Item.CollectionPath)
                                             .GetDocument(itemId)
                                             .UpdateDataAsync(nameof(Item.Title), title,
                                                              nameof(Item.Image), image,
                                                              nameof(Item.Comment), comment,
                                                              nameof(Item.Timestamp), DateTime.Now.Ticks)
                                             .ConfigureAwait(false);
                }

                _updateCompletedNotifier.OnNext(Unit.Default);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                _updateErrorNotifier.OnNext(e.Message);
            }
        }

        public async Task DeleteAsync(string itemId)
        {
            try
            {
                using (_deletingNotifier.ProcessStart())
                {
                    await CrossCloudFirestore.Current
                                             .Instance
                                             .GetCollection(Item.CollectionPath)
                                             .GetDocument(itemId)
                                             .DeleteDocumentAsync()
                                             .ConfigureAwait(false);
                }

                _deleteCompletedNotifier.OnNext(Unit.Default);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                _deleteErrorNotifier.OnNext(e.Message);
            }
        }
    }
}
