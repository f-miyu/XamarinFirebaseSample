using System;
using System.Threading.Tasks;
using Plugin.FirebaseAuth;
using XamarinFirebaseSample.Models;
using Plugin.CloudFirestore;
using System.Reactive.Subjects;
using Reactive.Bindings;
using XamarinFirebaseSample.Helpers;
using Xamarin.Forms;
using Prism.Mvvm;
using System.Reactive.Linq;
using Plugin.CloudFirestore.Extensions;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
using Reactive.Bindings.Notifiers;

namespace XamarinFirebaseSample.Services
{
    [Singleton]
    public class AccountService : IAccountService
    {
        private readonly IAuth _firebaseAuth;
        private readonly IFirestore _firestore;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly ReactivePropertySlim<bool> _isInitialized = new ReactivePropertySlim<bool>();
        public ReadOnlyReactivePropertySlim<bool> IsInitialized { get; }

        private readonly ReactivePropertySlim<bool> _isLoggedIn = new ReactivePropertySlim<bool>();
        public ReadOnlyReactivePropertySlim<bool> IsLoggedIn { get; }

        private readonly ReactivePropertySlim<string> _userId = new ReactivePropertySlim<string>();
        public ReadOnlyReactivePropertySlim<string> UserId { get; }

        private readonly ReactivePropertySlim<string> _userName = new ReactivePropertySlim<string>();
        public ReadOnlyReactivePropertySlim<string> UserName { get; }

        private readonly ReactivePropertySlim<string> _userImage = new ReactivePropertySlim<string>();
        public ReadOnlyReactivePropertySlim<string> UserImage { get; }

        private readonly ReactivePropertySlim<int> _contributionCount = new ReactivePropertySlim<int>();
        public ReadOnlyReactivePropertySlim<int> ContributionCount { get; }

        public AccountService()
        {
            _firebaseAuth = CrossFirebaseAuth.Current.Instance;
            _firestore = CrossCloudFirestore.Current.Instance;

            IsInitialized = _isInitialized.ToReadOnlyReactivePropertySlim();
            IsLoggedIn = _isLoggedIn.ToReadOnlyReactivePropertySlim();
            UserId = _userId.ToReadOnlyReactivePropertySlim();
            UserName = _userName.ToReadOnlyReactivePropertySlim();
            UserImage = _userImage.ToReadOnlyReactivePropertySlim();
            ContributionCount = _contributionCount.ToReadOnlyReactivePropertySlim();

            UserId.Select(userId => string.IsNullOrEmpty(userId) ?
                                    Observable.Return<User>(null) :
                                    _firestore.GetCollection(User.CollectionPath)
                                              .GetDocument(userId)
                                              .AsObservable()
                                              .Select(d => d.ToObject<User>()))
                  .Switch()
                  .Subscribe(user =>
                  {
                      if (user != null)
                      {
                          _userName.Value = user.Name;
                          _userImage.Value = user.Image;
                          _contributionCount.Value = user.ContributionCount;
                      }
                      else
                      {
                          _userName.Value = null;
                          _userImage.Value = null;
                          _contributionCount.Value = 0;
                      }
                  })
                  .AddTo(_disposables);
        }

        public async Task Initialize()
        {
            if (IsInitialized.Value) return;

            Plugin.FirebaseAuth.IListenerRegistration registration = null;
            try
            {
                var tcs = new TaskCompletionSource<string>();

                registration = _firebaseAuth.AddAuthStateChangedListener(auth =>
                {
                    tcs.TrySetResult(auth?.CurrentUser?.Uid);
                });

                var userId = await tcs.Task.ConfigureAwait(false);

                if (userId != null)
                {
                    var reference = _firestore.GetCollection(User.CollectionPath)
                                              .GetDocument(userId);

                    var document = await reference.GetDocumentAsync().ConfigureAwait(false);

                    if (document.Exists)
                    {
                        var user = document.ToObject<User>();

                        _userId.Value = user.Id;
                        _userName.Value = user.Name;
                        _userImage.Value = user.Image;
                        _contributionCount.Value = user.ContributionCount;
                        _isLoggedIn.Value = true;
                    }
                    else
                    {
                        _userId.Value = null;
                        _userName.Value = null;
                        _userImage.Value = null;
                        _contributionCount.Value = 0;
                        _isLoggedIn.Value = false;
                    }
                }
                else
                {
                    _userId.Value = null;
                    _userName.Value = null;
                    _userImage.Value = null;
                    _contributionCount.Value = 0;
                    _isLoggedIn.Value = false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            finally
            {
                registration?.Remove();
                _isInitialized.Value = true;
            }
        }

        public async Task LoginWithGoogleAsync(string idToken, string accessToken)
        {
            var credential = CrossFirebaseAuth.Current
                                              .GoogleAuthProvider
                                              .GetCredential(idToken, accessToken);

            var result = await _firebaseAuth.SignInWithCredentialAsync(credential).ConfigureAwait(false);

            var authUser = result.User;

            var reference = _firestore.GetCollection(User.CollectionPath)
                                      .GetDocument(authUser.Uid);

            var document = await reference.GetDocumentAsync().ConfigureAwait(false);

            var user = document.ToObject<User>();

            if (user == null)
            {
                user = new User()
                {
                    Id = authUser.Uid,
                    Name = authUser.DisplayName,
                    Image = authUser.PhotoUrl?.AbsoluteUri
                };

                await reference.SetDataAsync(user).ConfigureAwait(false);
            }

            _userId.Value = user.Id;
            _userName.Value = user.Name;
            _userImage.Value = user.Image;
            _contributionCount.Value = user.ContributionCount;
            _isLoggedIn.Value = true;
        }

        public async Task LoginWithEmailAndPasswordAsync(string email, string password)
        {

            var result = await _firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);

            var authUser = result.User;

            var reference = _firestore.GetCollection(User.CollectionPath)
                                      .GetDocument(authUser.Uid);

            var document = await reference.GetDocumentAsync().ConfigureAwait(false);

            var user = document.ToObject<User>();

            if (user == null)
            {
                user = new User()
                {
                    Id = authUser.Uid,
                    Name = authUser.DisplayName,
                    Image = authUser.PhotoUrl?.AbsoluteUri
                };

                await reference.SetDataAsync(user).ConfigureAwait(false);
            }

            _userId.Value = user.Id;
            _userName.Value = user.Name;
            _userImage.Value = user.Image;
            _contributionCount.Value = user.ContributionCount;
            _isLoggedIn.Value = true;
        }

        public async Task SignupAsync(string email, string password, string name, string image = null)
        {

            var result = await _firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password).ConfigureAwait(false);

            var authUser = result.User;

            var reference = _firestore.GetCollection(User.CollectionPath)
                                      .GetDocument(authUser.Uid);

            var user = new User
            {
                Id = authUser.Uid,
                Name = name,
                Image = image
            };

            await reference.SetDataAsync(user).ConfigureAwait(false);

            _userId.Value = user.Id;
            _userName.Value = user.Name;
            _userImage.Value = user.Image;
            _contributionCount.Value = user.ContributionCount;
            _isLoggedIn.Value = true;
        }

        public void Logout()
        {
            _firebaseAuth.SignOut();

            _userId.Value = null;
            _userName.Value = null;
            _userImage.Value = null;
            _contributionCount.Value = 0;
            _isLoggedIn.Value = false;
        }

        public Task IncrementContributionCountAsync(int delta)
        {
            return _firestore.RunTransactionAsync(transaction =>
            {
                var document = _firestore.GetCollection(User.CollectionPath)
                                         .GetDocument(UserId.Value);

                var user = transaction.GetDocument(document).ToObject<User>();

                if (user != null)
                {
                    user.ContributionCount += delta;
                    transaction.UpdateData(document, user);
                }
            });
        }

        public void Close()
        {
            _disposables.Dispose();
        }
    }
}
