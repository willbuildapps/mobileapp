using System;
using System.Collections.Generic;
using Toggl.Multivac;

namespace Toggl.Foundation.Sync
{
    public sealed class TransitionHandlerProvider : ITransitionHandlerProvider, ITransitionConfigurator
    {
        private readonly Dictionary<IStateResult, (Type, TransitionHandler)> transitionHandlers
            = new Dictionary<IStateResult, (Type, TransitionHandler)>();

        public void ConfigureTransition(IStateResult result, ISyncState state)
        {
            Ensure.Argument.IsNotNull(result, nameof(result));
            Ensure.Argument.IsNotNull(state, nameof(state));

            transitionHandlers.Add(result, (state.GetType(), _ => state.Start()));
        }

        public void ConfigureTransition<T>(StateResult<T> result, ISyncState<T> state)
        {
            Ensure.Argument.IsNotNull(result, nameof(result));
            Ensure.Argument.IsNotNull(state, nameof(state));

            transitionHandlers.Add(
                result,
                (state.GetType(), t => state.Start(((Transition<T>)t).Parameter))
            );
        }

        public TransitionHandler GetTransitionHandler(IStateResult result)
        {
            Ensure.Argument.IsNotNull(result, nameof(result));

            if (transitionHandlers.TryGetValue(result, out var tuple))
            {
                Console.WriteLine($@"starting state: {tuple.Item1.Name}");

                return tuple.Item2;
            }

            return null;
        }
    }
}
