namespace Day06Demo.Services;

public class MockDashboardService : IDashboardService
{
    public async Task<DashboardData> GetDashboardDataAsync()
    {
        // Simulate network delay
        await Task.Delay(300);

        return new DashboardData(
            NotificationCount: 5,
            ProductCount: 128,
            UserName: "Alice Johnson");
    }
}
