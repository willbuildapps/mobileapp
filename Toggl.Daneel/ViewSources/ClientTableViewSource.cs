using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Foundation;
using Toggl.Daneel.Cells;
using Toggl.Daneel.Views.Client;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class ClientTableViewSource : ListTableViewSource<SelectableClientBaseViewModel>
    {
        public IObservable<SelectableClientBaseViewModel> ClientSelected
            => Observable
                .FromEventPattern<SelectableClientBaseViewModel>(e => OnItemTapped += e, e => OnItemTapped -= e)
                .Select(e => e.EventArgs);

        private const int rowHeight = 48;

        public ClientTableViewSource() : base(ImmutableArray<SelectableClientBaseViewModel>.Empty)
        {
            ConfigureCell = configureCell;
        }

        public void SetNewClients(IEnumerable<SelectableClientBaseViewModel> clients)
        {
            items = clients.ToImmutableList();
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => rowHeight;

        private UITableViewCell configureCell(ListTableViewSource<SelectableClientBaseViewModel> source,
            UITableView tableView, NSIndexPath indexPath, SelectableClientBaseViewModel model)
        {
            var identifier = model is SelectableClientCreationViewModel ? CreateClientViewCell.Identifier : ClientViewCell.Identifier;
            var cell = tableView.DequeueReusableCell(identifier) as BaseTableViewCell<SelectableClientBaseViewModel>;
            cell.Item = model;
            return cell;
        }
    }
}
