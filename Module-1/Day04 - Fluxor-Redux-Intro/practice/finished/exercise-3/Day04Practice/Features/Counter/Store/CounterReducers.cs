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

    [ReducerMethod]
    public static CounterState ReduceDecrementCounterAction(
        CounterState state,
        DecrementCounterAction action)
    {
        return state with { Count = state.Count - 1 };
    }

    [ReducerMethod]
    public static CounterState ReduceResetCounterAction(
        CounterState state,
        ResetCounterAction action)
    {
        return state with { Count = 0 };
    }
}
