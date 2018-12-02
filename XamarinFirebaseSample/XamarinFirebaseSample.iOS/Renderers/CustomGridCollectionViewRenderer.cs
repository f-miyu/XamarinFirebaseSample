using System;
using System.Collections.Specialized;
using System.Linq;
using AiForms.Renderers;
using AiForms.Renderers.iOS;
using Xamarin.Forms;
using XamarinFirebaseSample.Controls;
using XamarinFirebaseSample.iOS.Renderers;

[assembly: ExportRenderer(typeof(CustomGridCollectionView), typeof(CustomGridCollectionViewRenderer))]
namespace XamarinFirebaseSample.iOS.Renderers
{
    public class CustomGridCollectionViewRenderer : GridCollectionViewRenderer
    {
        protected override void UpdateItems(NotifyCollectionChangedEventArgs e, int section, bool resetWhenGrouped, bool forceReset = false)
        {
            if (!Control.IndexPathsForVisibleItems.Any() ||
               (e.Action == NotifyCollectionChangedAction.Remove && Control.IndexPathsForVisibleItems.Count() == 1))
            {
                forceReset = true;
            }
            base.UpdateItems(e, section, resetWhenGrouped, forceReset);
        }
    }
}
