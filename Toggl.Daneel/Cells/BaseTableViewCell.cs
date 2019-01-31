using System;
using Foundation;
using Toggl.Daneel.ViewSources;
using UIKit;

namespace Toggl.Daneel.Cells
{
    public abstract class BaseTableViewCell<TModel> : UITableViewCell
    {
        private TModel item;
        public TModel Item
        {
            get => item;
            set
            {
                item = value;
                UpdateView();
            }
        }

        protected BaseTableViewCell()
        {
        }

        protected BaseTableViewCell(IntPtr handle)
            : base(handle)
        {
        }

        protected abstract void UpdateView();

        public static SectionedListTableViewSource<TModel>.CellConfiguration CellConfiguration(string cellIdentifier)
        {
            return (source, tableView, indexPath, model) =>
            {
                var cell = tableView.DequeueReusableCell(cellIdentifier, indexPath) as BaseTableViewCell<TModel>;
                cell.Item = model;
                return cell;
            };
        }
    }
}
