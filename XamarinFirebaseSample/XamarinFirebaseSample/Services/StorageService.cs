using System;
using XamarinFirebaseSample.Helpers;
using Prism.DryIoc;
using Plugin.FirebaseStorage;
using System.Threading.Tasks;
using System.IO;
namespace XamarinFirebaseSample.Services
{
    [Singleton]
    public class StorageService : IStorageService
    {
        private readonly IStorage _storage;

        public StorageService()
        {
            _storage = CrossFirebaseStorage.Current.Instance;
        }

        public async Task<Uri> UploadImage(Stream image, string path)
        {
            var metadata = new MetadataChange
            {
                ContentType = "image/jpeg"
            };

            var reference = _storage.RootReference.GetChild(path);

            await reference.PutStreamAsync(image, metadata).ConfigureAwait(false);

            return await reference.GetDownloadUrlAsync().ConfigureAwait(false);
        }
    }
}
