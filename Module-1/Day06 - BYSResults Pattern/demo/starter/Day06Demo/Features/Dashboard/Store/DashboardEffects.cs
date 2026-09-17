using Fluxor;
using Day06Demo.Services;

namespace Day06Demo.Features.Dashboard.Store;

public class DashboardEffects(IDashboardService dashboardService)
{
    [EffectMethod]
    public async Task HandleLoadDashboardAction(
        LoadDashboardAction action, IDispatcher dispatcher)
    {
        // Demo B — Step 6 [CODE LIVE]: Replace this try/catch with Result<T> checking.
        //
        // After IDashboardService returns Task<Result<DashboardData>>, change this to:
        //     var result = await dashboardService.GetDashboardDataAsync();
        //     if (result.IsSuccess)
        //     {
        //         DashboardData data = result.Value!;   // ! -- IsSuccess guarantees non-null
        //         dispatcher.Dispatch(new LoadDashboardSuccessAction(
        //             data.NotificationCount, data.ProductCount, data.UserName));
        //     }
        //     else
        //     {
        //         var errors = result.Errors.Select(e => e.ToString()).ToList();
        //         dispatcher.Dispatch(new LoadDashboardFailureAction(
        //             string.Join("; ", errors)));
        //     }
        try
        {
            var data = await dashboardService.GetDashboardDataAsync();
            dispatcher.Dispatch(new LoadDashboardSuccessAction(
                data.NotificationCount, data.ProductCount, data.UserName));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadDashboardFailureAction(ex.Message));
        }
    }
}
