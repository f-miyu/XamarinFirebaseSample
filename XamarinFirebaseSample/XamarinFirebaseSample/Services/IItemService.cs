using System;
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
        IObservable<string> LoadErrorNotifier { get; }
        Task LoadAsync(string id);
        Task LikeOrUnlikeAsync();
        Task LikeAsync();
        Task UnlikeAsync();
    }
}
