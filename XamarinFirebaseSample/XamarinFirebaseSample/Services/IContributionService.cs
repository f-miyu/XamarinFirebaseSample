using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace XamarinFirebaseSample.Services
{
    public interface IContributionService
    {
        ReactivePropertySlim<Stream> ItemImage { get; }
        ReactivePropertySlim<string> ItemTitle { get; }
        ReactivePropertySlim<string> ItemComment { get; }
        ReadOnlyReactivePropertySlim<bool> CanContribute { get; }
        IObservable<bool> ContributingNotifier { get; }
        IObservable<string> ContributeErrorNotifier { get; }
        IObservable<Unit> ContributeCompletedNotifier { get; }
        Task SelectImage();
        Task Contribute();
    }
}
