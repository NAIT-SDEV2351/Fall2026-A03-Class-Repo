namespace Day04Demo.Features.Counter.Store;

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
    public static CounterState ReduceSetCounterAction(
        CounterState state,
        SetCounterAction action)
    {
        return state with { Count = action.NewCount };
    }

    [ReducerMethod]
    public static CounterState ReduceDecrementCounterAction(
        CounterState state,
        DecrementCounterAction action)
    {
        return state with { Count = state.Count - 1 };
    }
}