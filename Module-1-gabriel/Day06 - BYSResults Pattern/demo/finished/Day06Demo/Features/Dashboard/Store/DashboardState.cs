namespace Day06Demo.Features.Dashboard.Store;

// The store slice — one immutable record shared by all three cards. The init-only
// properties mean nothing mutates it in place; a reducer produces a NEW state via
// `with`. IsLoading / ErrorMessage drive the loading and error UI the cards show.
public record DashboardState
{
    public int NotificationCount { get; init; }
    public int ProductCount { get; init; }
    public string UserName { get; init; } = string.Empty;
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
}
