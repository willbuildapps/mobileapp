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
    public sealed class CountryTableViewSource : SectionedListTableViewSource<SelectableCountryViewModel>
    {
        private const int rowHeight = 48;

        public CountryTableViewSource()
            : base(ImmutableArray<SelectableCountryViewModel>.Empty)
        {
            ConfigureCell = configureCell;
        }

        public void SetNewCountries(IEnumerable<SelectableCountryViewModel> countries)
            => SetItems(countries.ToImmutableList());

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => rowHeight;

        private UITableViewCell configureCell(SectionedListTableViewSource<SelectableCountryViewModel> source,
            UITableView tableView, NSIndexPath indexPath, SelectableCountryViewModel model)
        {
            var cell = tableView.DequeueReusableCell(CountryViewCell.Identifier) as CountryViewCell;
            cell.Item = model;
            return cell;
        }
    }
}
