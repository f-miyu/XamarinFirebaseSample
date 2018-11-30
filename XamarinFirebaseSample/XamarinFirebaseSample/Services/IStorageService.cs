using System;
using System.IO;
using System.Threading.Tasks;

namespace XamarinFirebaseSample.Services
{
    public interface IStorageService
    {
        Task<Uri> UploadImage(Stream image, string path);
    }
}
