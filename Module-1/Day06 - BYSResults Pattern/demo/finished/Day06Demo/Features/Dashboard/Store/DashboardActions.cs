namespace Day06Demo.Features.Dashboard.Store;

// The request / success / failure action triad. The component dispatches the request
// (LoadDashboardAction); the effect dispatches exactly ONE of the other two when the
// async work finishes. Records are immutable messages — the constructor parameters are
// the payload the reducers copy into state.
public record LoadDashboardAction;

public record LoadDashboardSuccessAction(
    int NotificationCount,
    int ProductCount,
    string UserName);

public record LoadDashboardFailureAction(string ErrorMessage);
