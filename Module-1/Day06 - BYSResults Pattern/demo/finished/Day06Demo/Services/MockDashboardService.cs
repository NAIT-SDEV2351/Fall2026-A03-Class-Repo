using BYSResults;

namespace Day06Demo.Services;

// Demo B: returns Task<Result<DashboardData>> using the BYSResult pattern.
// The Result container carries either a value (.WithValue) or accumulated
// errors (.AddError) — no exceptions for business outcomes.
public class MockDashboardService : IDashboardService
{
    public async Task<Result<DashboardData>> GetDashboardDataAsync()
    {
        // Simulate network delay
        await Task.Delay(300);

        var result = new Result<DashboardData>();
        return result.WithValue(new DashboardData(
            NotificationCount: 5,
            ProductCount: 128,
            UserName: "Alice Johnson"));
    }
}
