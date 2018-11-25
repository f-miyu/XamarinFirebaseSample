using System;
using System.ComponentModel;
namespace XamarinFirebaseSample.Models
{
    public class Item : INotifyPropertyChanged
    {
        public static string CollectionPath = "items";

        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; }

        public string Image { get; set; }

        public long Timestamp { get; set; }
    }
}
