using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using Toggl.Foundation.MvvmCross.ViewModels.Reports;
using Toggl.Giskard.TemplateSelectors;
using Toggl.Giskard.Views;
using Android.Widget;
using Toggl.Foundation.Reports;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.ViewHolders;
using static Toggl.Giskard.Resource.Id;

namespace Toggl.Giskard.Adapters
{
    public sealed class ReportsRecyclerAdapter : RecyclerView.Adapter
    {
        private readonly int lastItemCellHeight;
        private readonly int normalItemCellHeight;

        public const int WorkspaceName = 0;
        public const int Header = 1;
        public const int Item = 2;

        private const int workspaceNameCellIndex = 0;
        private const int summaryCardCellIndex = 1;
        private const int headerItemsCount = 2;

        private ReportsViewModel ViewModel { get; }

        public ReportsRecyclerAdapter(Context context, ReportsViewModel viewModel)
        {
            ViewModel = viewModel;
            lastItemCellHeight = 72.DpToPixels(context);
            normalItemCellHeight = 48.DpToPixels(context);
        }

        public ReportsRecyclerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layoutInflater = LayoutInflater.From(parent.Context);
            switch (viewType)
            {
                case WorkspaceName:
                    var workpaceNameCell = layoutInflater.Inflate(Resource.Layout.ReportsActivityWorkspaceName, parent, false);
                    return new ReportsWorkspaceNameViewHolder(workpaceNameCell);

                case Header:
                    var headerCellView = layoutInflater.Inflate(Resource.Layout.ReportsActivityHeader, parent, false);
                    return new ReportsHeaderCellViewHolder(headerCellView);

                case Item:
                    var itemCellView = layoutInflater.Inflate(Resource.Layout.ReportsActivityItem, parent, false);
                    return new ReportsItemCellViewHolder(itemCellView, lastItemCellHeight, normalItemCellHeight);

                default:
                    throw new InvalidOperationException($"Invalid view type {viewType}");
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            switch (holder)
            {
                    case ReportsItemCellViewHolder reportsViewHolder:
                        reportsViewHolder.IsLastItem = position == ItemCount - 1;
                        reportsViewHolder.RecalculateSize();
                        break;

                    case ReportsWorkspaceNameViewHolder reportsWorkspaceHolder:
                        reportsWorkspaceHolder.Item = ViewModel.WorkspaceName;
                        break;

                    case ReportsHeaderCellViewHolder reportsSummaryHolder:
                        reportsSummaryHolder.Item = ViewModel;
                        break;

                    default:
                        throw new InvalidOperationException($"Tried to bind unexpected viewholder {holder?.GetType().Name ?? "null"}");
            }
        }

        public override int ItemCount
            => headerItemsCount + (ViewModel?.Segments.Count ?? 0);


        public override int GetItemViewType(int position)
        {
            switch (position)
            {
                case workspaceNameCellIndex:
                    return WorkspaceName;

                case summaryCardCellIndex:
                    return Header;

                default:
                    return Item;
            }
        }
    }
}
