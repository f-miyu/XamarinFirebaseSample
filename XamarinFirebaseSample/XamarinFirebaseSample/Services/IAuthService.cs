using System;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace XamarinFirebaseSample.Services
{
    public interface IAuthService
    {
        void OnPageLoading(Uri uri);
        Task<(string IdToken, string AccessToken)> LoginWithGoogle();
    }
}
