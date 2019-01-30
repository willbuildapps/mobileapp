using System;
using System.Collections.Immutable;
using Foundation;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public class ListCollectionViewSource<TModel> : UICollectionViewSource
    {
        internal IImmutableList<TModel> items;

        public EventHandler<TModel> OnItemTapped { get; set; }
        public Func<ListCollectionViewSource<TModel>, UICollectionView, NSIndexPath, TModel, UICollectionViewCell> ConfigureCell;

        public ListCollectionViewSource(IImmutableList<TModel> items)
        {
            this.items = items;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return ConfigureCell(this, collectionView, indexPath, items[indexPath.Row]);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            collectionView.DeselectItem(indexPath, true);
            OnItemTapped.Invoke(this, items[indexPath.Row]);
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
            => items.Count;
    }
}
