using Fluxor;

namespace Day06Demo.Features.Dashboard.Store;

public static class DashboardReducers
{
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
