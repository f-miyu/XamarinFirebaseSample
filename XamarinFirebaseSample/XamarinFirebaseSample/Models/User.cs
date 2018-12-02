using System;
using System.ComponentModel;
using Plugin.CloudFirestore.Attributes;
namespace XamarinFirebaseSample.Models
{
    public class User : INotifyPropertyChanged
    {
        public static string CollectionPath = "users";

        public event PropertyChangedEventHandler PropertyChanged;

        [Id]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public int ContributionCount { get; set; }

        public void CopyTo(User user)
        {
            user.Id = Id;
            user.Name = Name;
            user.Image = Image;
        }
    }
}
