using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using Toggl.Daneel.Views.CountrySelection;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class CountryTableViewSource : ListTableViewSource<SelectableCountryViewModel>
    {
        public IObservable<SelectableCountryViewModel> CountrySelected
            => Observable
                .FromEventPattern<SelectableCountryViewModel>(e => OnItemTapped += e, e => OnItemTapped -= e)
                .Select(e => e.EventArgs);

        private const int rowHeight = 48;

        public CountryTableViewSource()
            : base(ImmutableArray<SelectableCountryViewModel>.Empty)
        {
            ConfigureCell = configureCell;
        }

        public void SetNewCountries(IEnumerable<SelectableCountryViewModel> countries)
        {
            items = countries.ToImmutableList();
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => rowHeight;

        private UITableViewCell configureCell(ListTableViewSource<SelectableCountryViewModel> source,
            UITableView tableView, NSIndexPath indexPath, SelectableCountryViewModel model)
        {
            var cell = tableView.DequeueReusableCell(CountryViewCell.Identifier) as CountryViewCell;
            cell.Item = model;
            return cell;
        }
    }
}
