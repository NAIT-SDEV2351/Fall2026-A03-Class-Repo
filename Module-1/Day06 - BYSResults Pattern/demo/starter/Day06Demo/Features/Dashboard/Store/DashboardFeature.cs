using Fluxor;

namespace Day06Demo.Features.Dashboard.Store;

public class DashboardFeature : Feature<DashboardState>
{
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
