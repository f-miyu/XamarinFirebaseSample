using System;
using System.Threading.Tasks;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using XamarinFirebaseSample.Helpers;
using XamarinFirebaseSample.Models;
using Reactive.Bindings.Notifiers;
using System.Reactive.Subjects;

namespace XamarinFirebaseSample.Services
{
    [Singleton]
    public class CurrentUserService
    {
        private readonly IAuthService _authService;

        public CurrentUserService(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task LoginWithGoogle()
        {
            try
            {
                var (idToken, accessToken) = await _authService.LoginWithGoogle();

                if (idToken != null)
                {
                    var credential = CrossFirebaseAuth.Current
                                                      .GoogleAuthProvider
                                                      .GetCredential(idToken, accessToken);

                    var result = await CrossFirebaseAuth.Current
                                                        .Instance
                                                        .SignInWithCredentialAsync(credential)
                                                        .ConfigureAwait(false);

                    var authUser = result.User;

                    var reference = CrossCloudFirestore.Current
                                                       .Instance
                                                       .GetCollection(User.CollectionPath)
                                                       .GetDocument(authUser.Uid);

                    var document = await reference.GetDocumentAsync().ConfigureAwait(false);

                    if (!document.Exists)
                    {
                        var user = new User
                        {
                            Name = authUser.DisplayName,
                            Image = authUser.PhotoUrl.ToString()
                        };

                        await reference.SetDataAsync(user).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }
    }
}
