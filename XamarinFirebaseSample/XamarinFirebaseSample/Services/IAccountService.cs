using System;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace XamarinFirebaseSample.Services
{
    public interface IAccountService
    {
        ReadOnlyReactivePropertySlim<bool> IsInitialized { get; }
        ReadOnlyReactivePropertySlim<bool> IsLoggedIn { get; }
        IObservable<string> LoginErrorNotifier { get; }
        Task LoginWithGoogle();
    }
}
