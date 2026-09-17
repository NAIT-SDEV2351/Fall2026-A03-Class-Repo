using Fluxor;

namespace Day06Demo.Features.Dashboard.Store;

public class DashboardFeature : Feature<DashboardState>
{
    // GetName() identifies this slice in the store (and Redux DevTools). GetInitialState()
    // is the state before any action runs — these zeros and blank name are exactly what the
    // cards display on first paint, until LoadDashboardSuccessAction fills them in.
    public override string GetName() => "Dashboard";

    protected override DashboardState GetInitialState()
    {
        return new DashboardState
        {
            NotificationCount = 0,
            ProductCount = 0,
            UserName = string.Empty,
            IsLoading = false,
            ErrorMessage = null
        };
    }
}
