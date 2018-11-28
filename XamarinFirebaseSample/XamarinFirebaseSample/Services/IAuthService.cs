using System;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace XamarinFirebaseSample.Services
{
    public interface IAuthService
    {
        Task<(string IdToken, string AccessToken)> LoginWithGoogle();
        void OnPageLoading(Uri uri);
    }
}
