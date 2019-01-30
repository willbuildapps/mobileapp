using System;
using System.Reactive.Linq;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation.MvvmCross.Reactive;

namespace Toggl.Daneel.Extensions.Reactive
{
    public static class SectionedListTableViewSourceExtensions
    {
        public static IObservable<TModel> ModelSelected<TModel>(
            this IReactive<SectionedListTableViewSource<TModel>> reactive)
            => Observable
                .FromEventPattern<TModel>(e => reactive.Base.OnItemTapped += e, e => reactive.Base.OnItemTapped -= e)
                .Select(e => e.EventArgs);
    }
}
