using System;
using System.Reactive;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace XamarinFirebaseSample.Services
{
    public interface IGoogleLoginService
    {
        ReadOnlyReactivePropertySlim<bool> IsLoggingIn { get; }
        IObservable<string> LoginErrorNotifier { get; }
        IObservable<Unit> LoginCompletedNotifier { get; }
        Task Login();
    }
}
