using System;
using System.Reactive;
using System.Threading.Tasks;

namespace XamarinFirebaseSample.Services
{
    public interface IItemRepositoryService
    {
        IObservable<bool> AddingNotifier { get; }
        IObservable<string> AddErrorNotifier { get; }
        IObserver<Unit> AddCompletedNotifier { get; }
        IObservable<bool> UpdatingNotifier { get; }
        IObservable<string> UpdateErrorNotifier { get; }
        IObserver<Unit> UpdateCompletedNotifier { get; }
        IObservable<bool> DeletingNotifier { get; }
        IObservable<string> DeleteErrorNotifier { get; }
        IObserver<Unit> DeleteCompletedNotifier { get; }
        Task AddAsync(string title, string image, string comment, string ownerId);
        Task UpdateAsync(string itemId, string title, string image, string comment);
        Task DeleteAsync(string itemId);
    }
}
