using Fluxor;

namespace Day06Demo.Features.Dashboard.Store;

public static class DashboardReducers
{
    // Reducers are pure: (currentState, action) → NEW state. No I/O, no side effects.
    // `state with { ... }` copies the record and overrides only the named properties —
    // every other field is carried forward unchanged. One [ReducerMethod] per action;
    // Fluxor routes each dispatched action to the reducer whose parameter type matches.
    [ReducerMethod]
    public static DashboardState ReduceLoadDashboardAction(
        DashboardState state, LoadDashboardAction action)
    {
        return state with { IsLoading = true, ErrorMessage = null };
    }

    [ReducerMethod]
    public static DashboardState ReduceLoadDashboardSuccessAction(
        DashboardState state, LoadDashboardSuccessAction action)
    {
        return state with
        {
            NotificationCount = action.NotificationCount,
            ProductCount = action.ProductCount,
            UserName = action.UserName,
            IsLoading = false,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static DashboardState ReduceLoadDashboardFailureAction(
        DashboardState state, LoadDashboardFailureAction action)
    {
        return state with { IsLoading = false, ErrorMessage = action.ErrorMessage };
    }
}
