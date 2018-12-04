using System;
using System.Reactive;
using System.Threading.Tasks;
using Reactive.Bindings;
using XamarinFirebaseSample.Models;

namespace XamarinFirebaseSample.Services
{
    public interface IItemService
    {
        ReadOnlyReactivePropertySlim<Item> Item { get; }
        ReadOnlyReactivePropertySlim<User> Owner { get; }
        ReadOnlyReactivePropertySlim<bool> IsLiked { get; }
        ReadOnlyReactivePropertySlim<bool> IsOwner { get; }
        ReadOnlyReactivePropertySlim<bool> IsLoaded { get; }
        ReadOnlyReactivePropertySlim<bool> IsDeleting { get; }
        IObservable<string> LoadErrorNotifier { get; }
        IObservable<string> DeleteErrorNotifier { get; }
        IObservable<Unit> DeleteCompletedNotifier { get; }
        Task LoadAsync(string id);
        Task LikeOrUnlikeAsync();
        Task LikeAsync();
        Task UnlikeAsync();
        Task DeleteAsync();
    }
}
