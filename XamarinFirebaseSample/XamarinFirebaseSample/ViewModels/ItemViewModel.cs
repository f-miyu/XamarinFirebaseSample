using System;
using XamarinFirebaseSample.Models;
using Reactive.Bindings;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;
namespace XamarinFirebaseSample.ViewModels
{
    public class ItemViewModel : IDisposable
    {
        private readonly Item _item;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ReadOnlyReactivePropertySlim<string> Id { get; }
        public ReadOnlyReactivePropertySlim<string> Title { get; }
        public ReadOnlyReactivePropertySlim<string> Image { get; }

        public ItemViewModel(Item item)
        {
            _item = item;

            Id = _item.ObserveProperty(x => x.Id)
                      .ToReadOnlyReactivePropertySlim()
                      .AddTo(_disposables);

            Title = _item.ObserveProperty(x => x.Title)
                         .ToReadOnlyReactivePropertySlim()
                         .AddTo(_disposables);

            Image = _item.ObserveProperty(x => x.Image)
                         .ToReadOnlyReactivePropertySlim()
                         .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
