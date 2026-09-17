namespace Day06Demo.Services;

using BYSResults;

public class MockDashboardService : IDashboardService
{
    public async Task<Result<DashboardData>> GetDashboardDataAsync()
    {
        // Simulate network delay
        await Task.Delay(300);

        var result = new Result<DashboardData>();
        result.WithValue(new DashboardData(
            NotificationCount: 5,
            ProductCount: 128,
            UserName: "Alice Johnson"));

        // result.AddError(new Error("error message"));
        
        return result;
    }
}
