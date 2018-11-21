using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using MvvmCross.Commands;
using MvvmCross.Platforms.Ios.Binding.Views;
using Toggl.Daneel.Views;
using Toggl.Foundation.MvvmCross.ViewModels.ReportsCalendar;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class ReportsCalendarCollectionViewSource : UICollectionViewDataSource
    {
        private const string cellIdentifier = nameof(ReportsCalendarViewCell);

        private ImmutableList<ReportsCalendarPageViewModel> months;
        private UICollectionView collectionView;
        private readonly ISubject<ReportsCalendarDayViewModel> itemTappedSubject = new Subject<ReportsCalendarDayViewModel>();

        public IObservable<ReportsCalendarDayViewModel> ItemTapped => itemTappedSubject.AsObservable();

        public ReportsCalendarCollectionViewSource(UICollectionView collectionView)
        {
            this.collectionView = collectionView;
            months = new List<ReportsCalendarPageViewModel>().ToImmutableList();
            collectionView.RegisterNibForCell(ReportsCalendarViewCell.Nib, cellIdentifier);
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {

            var item = months[indexPath.Section].Days[(int)indexPath.Item];
            var cell = collectionView.DequeueReusableCell(cellIdentifier, indexPath) as UICollectionViewCell;

            if (cell is ReportsCalendarViewCell calendarCell)
            {
                calendarCell.BackgroundColor = UIColor.Green;
                calendarCell.DataContext = item;
                calendarCell.CellTapped = () =>
                {
                    itemTappedSubject.OnNext(item);
                };
            }

            return cell;
        }

        public override nint NumberOfSections(UICollectionView collectionView)
            => months.Count;

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
            => months[(int)section].Days.Count;

        public void CollectionChanged(IImmutableList<ReportsCalendarPageViewModel> months)
        {
            this.months = months.ToImmutableList();
            collectionView.ReloadData();
        }
    }
}
