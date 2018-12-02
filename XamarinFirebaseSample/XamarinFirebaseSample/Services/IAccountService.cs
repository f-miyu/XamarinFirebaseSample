using System;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace XamarinFirebaseSample.Services
{
    public interface IAccountService
    {
        ReadOnlyReactivePropertySlim<bool> IsInitialized { get; }
        ReadOnlyReactivePropertySlim<bool> IsLoggedIn { get; }
        ReadOnlyReactivePropertySlim<string> UserId { get; }
        ReadOnlyReactivePropertySlim<string> UserName { get; }
        ReadOnlyReactivePropertySlim<string> UserImage { get; }
        ReadOnlyReactivePropertySlim<int> ContributionCount { get; }
        Task Initialize();
        Task LoginWithGoogleAsync(string idToken, string accessToken);
        Task LoginWithEmailAndPasswordAsync(string email, string password);
        Task SignupAsync(string email, string password, string name, string image = null);
        void Logout();
        Task IncrementContributionCountAsync(int delta);
        void Close();
    }
}
