namespace Day04Demo.Features.Counter.Store;

using Fluxor;

// Reducers are PURE: (currentState, action) → NEW state. No I/O, no mutation —
// `state with { Count = ... }` copies the record and overrides only Count, carrying every
// other field forward. Fluxor routes each dispatched action to the [ReducerMethod] whose
// parameter type matches it.
//
// Naming convention: a reducer method is named Reduce<FullActionName> — put
// "Reduce" in front of the whole action name (e.g. IncrementCounterAction ->
// ReduceIncrementCounterAction). This is the standard Fluxor convention (Mark
// Morris's official docs): the "Reduce" prefix marks the method as a reducer,
// and keeping the action's full name shows exactly which action it handles.
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
    public static CounterState ReduceSetCounterAction(
        CounterState state,
        SetCounterAction action)
    {
        return state with { Count = action.NewCount };
    }
}
