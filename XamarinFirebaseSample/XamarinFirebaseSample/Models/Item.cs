using System;
using System.ComponentModel;
using Plugin.CloudFirestore.Attributes;

namespace XamarinFirebaseSample.Models
{
    public class Item : INotifyPropertyChanged
    {
        public static string CollectionPath = "items";

        public event PropertyChangedEventHandler PropertyChanged;

        [Id]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public long Timestamp { get; set; }

        public void CopyTo(Item item)
        {
            item.Id = Id;
            item.Title = item.Title;
            item.Image = item.Image;
            item.Timestamp = item.Timestamp;
        }
    }
}
