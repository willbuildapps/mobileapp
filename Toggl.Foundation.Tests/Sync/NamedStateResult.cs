using Toggl.Foundation.Sync;

namespace Toggl.Foundation.Tests.Sync
{
    internal struct NamedStateResult
    {
        public object State { get; }
        public IStateResult Result { get; }
        public string Name { get; }

        public NamedStateResult(object state, IStateResult result, string name)
        {
            State = state;
            Result = result;
            Name = name;
        }

        public override string ToString()
        {
            return $"{State.GetType().Name}.{Name}";
        }
    }
}
