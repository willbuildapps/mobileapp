using Toggl.Foundation.MvvmCross.Interfaces;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public abstract class SelectableTagBaseViewModel : IDiffable<SelectableTagBaseViewModel>
    {
        public string Name { get; }
        public bool Selected { get; set; }

        public SelectableTagBaseViewModel(string name, bool selected)
        {
            Ensure.Argument.IsNotNullOrWhiteSpaceString(name, nameof(name));
            Name = name;
            Selected = selected;
        }

        public override string ToString() => Name;

        public bool Equals(SelectableTagBaseViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name)
                && Selected == other.Selected;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SelectableTagBaseViewModel)obj);
        }

        public override int GetHashCode() => HashCode.From(Name ?? string.Empty, Selected);

        public long Identifier => Name.GetHashCode();
    }

    public sealed class SelectableTagViewModel : SelectableTagBaseViewModel
    {
        public long Id { get; }
        public string Workspace { get; }

        public SelectableTagViewModel(
            long id,
            string workspace,
            string name,
            bool selected
        ) : base(name, selected)
        {
            Ensure.Argument.IsNotNull(id, nameof(id));
            Ensure.Argument.IsNotNull(workspace, nameof(workspace));
            Id = id;
            Workspace = workspace;
        }
    }

    public sealed class SelectableTagCreationViewModel : SelectableTagBaseViewModel
    {
        public SelectableTagCreationViewModel(string name)
            : base(name, false)
        {
        }
    }
}
