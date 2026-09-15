namespace Day04Practice.Features.Counter.Store;

using Fluxor;

public static class CounterReducers
{
    [ReducerMethod]
    public static CounterState ReduceIncrementCounterAction(
        CounterState state,
        IncrementCounterAction action)
    {
        return state with { Count = state.Count + 1 };
    }

    // TODO: Add ReduceDecrementCounterAction
    // Pattern: return state with { Count = state.Count - 1 };

    // TODO: Add ReduceResetCounterAction
    // Pattern: return state with { Count = 0 };
}
