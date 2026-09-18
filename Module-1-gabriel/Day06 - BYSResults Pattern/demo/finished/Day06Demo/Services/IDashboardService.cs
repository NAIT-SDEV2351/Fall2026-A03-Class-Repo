using BYSResults;

namespace Day06Demo.Services;

public record DashboardData(int NotificationCount, int ProductCount, string UserName);

// Demo B: the service contract returns Result<DashboardData>. The caller
// (DashboardEffects) checks result.IsSuccess / result.IsFailure instead of
// catching exceptions for business-logic outcomes.
public interface IDashboardService
{
    Task<Result<DashboardData>> GetDashboardDataAsync();
}
