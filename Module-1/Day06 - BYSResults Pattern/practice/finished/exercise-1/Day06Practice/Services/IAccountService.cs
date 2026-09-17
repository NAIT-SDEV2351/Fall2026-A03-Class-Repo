namespace Day06Practice.Services;

public record AccountSummary(int OrderCount, int WishlistCount, string MembershipTier);

// NOTE: This practice version returns raw Task<T> for simplicity.
// The instructor demo (Day06Demo) wraps the return type in Result<T>
// using the BYSResult pattern — Task<Result<DashboardData>> — which
// provides structured success/failure handling. You adopt the
// Result<T> pattern in Practice 2 (ProductService).
public interface IAccountService
{
    Task<AccountSummary> GetAccountSummaryAsync();
}
