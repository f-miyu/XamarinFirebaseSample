using System;
using Plugin.CloudFirestore;
namespace XamarinFirebaseSample.Services
{
    public class ItemListService : IItemListService
    {
        private readonly IFirestore _firestore;

        public ItemListService()
        {
        }
    }
}
