using System;
using System.Collections.Generic;
using Plugin.CloudFirestore.Attributes;
namespace XamarinFirebaseSample.Models
{
    public class Like
    {
        public static string CollectionPath = "likes";

        [Id]
        public string ItemId { get; set; }

        public long Timestamp { get; set; }
    }
}
