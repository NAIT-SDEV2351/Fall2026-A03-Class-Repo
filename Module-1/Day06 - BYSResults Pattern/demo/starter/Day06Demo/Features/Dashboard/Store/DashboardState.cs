namespace Day06Demo.Features.Dashboard.Store;

public record DashboardState
{
    public int NotificationCount { get; init; }
    public int ProductCount { get; init; }
    public string UserName { get; init; } = string.Empty;
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
}
