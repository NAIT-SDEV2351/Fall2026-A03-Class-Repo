using Fluxor;

namespace Day06Practice.Features.Account.Store;

public class AccountFeature : Feature<AccountState>
{
    public override string GetName() => "Account";

    protected override AccountState GetInitialState()
    {
        return new AccountState
        {
            OrderCount = 0,
            WishlistCount = 0,
            MembershipTier = string.Empty,
            IsLoading = false,
            ErrorMessage = null
        };
    }
}
