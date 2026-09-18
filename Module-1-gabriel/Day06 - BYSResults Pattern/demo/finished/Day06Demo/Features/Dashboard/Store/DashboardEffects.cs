using Fluxor;
using Day06Demo.Services;

namespace Day06Demo.Features.Dashboard.Store;

public class DashboardEffects(IDashboardService dashboardService)
{
    // Effects own the async side-effect (the service call) and then dispatch a result
    // action back into the store — they never mutate state directly (reducers do that).
    [EffectMethod]
    public async Task HandleLoadDashboardAction(
        LoadDashboardAction action, IDispatcher dispatcher)
    {
        // Demo B: no try/catch. The service returns a Result<T>, so a business failure
        // is a value we inspect (IsSuccess / Errors), not an exception we catch. Reserve
        // exceptions for the truly exceptional; expected failures travel in the Result.
        var result = await dashboardService.GetDashboardDataAsync();

        if (result.IsSuccess)
        {
            // result.Value! — the IsSuccess guard guarantees non-null at runtime; the !
            // keeps the build warning-clean since the compiler can't infer that from the guard.
            DashboardData data = result.Value!;
            dispatcher.Dispatch(new LoadDashboardSuccessAction(
                data.NotificationCount, data.ProductCount, data.UserName));
        }
        else
        {
            // Failure path: flatten the accumulated Result errors into one message and
            // hand it to the store via the failure action; the cards then show it.
            var errors = result.Errors.Select(e => e.ToString()).ToList();
            dispatcher.Dispatch(new LoadDashboardFailureAction(
                string.Join("; ", errors)));
        }
    }
}
