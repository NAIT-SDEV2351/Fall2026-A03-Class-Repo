namespace Day06Demo.Features.Dashboard.Store;

public record LoadDashboardAction;

public record LoadDashboardSuccessAction(
    int NotificationCount,
    int ProductCount,
    string UserName);

public record LoadDashboardFailureAction(string ErrorMessage);
