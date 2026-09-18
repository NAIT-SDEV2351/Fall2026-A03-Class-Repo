namespace Day06Practice.Services;

// NOTE: Returns raw Task<AccountSummary> for simplicity. The instructor
// demo uses Task<Result<DashboardData>> with the BYSResult pattern.
public class MockAccountService : IAccountService
{
    public async Task<AccountSummary> GetAccountSummaryAsync()
    {
        await Task.Delay(300);

        return new AccountSummary(
            OrderCount: 12,
            WishlistCount: 5,
            MembershipTier: "Gold");
    }
}
