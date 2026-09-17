namespace Day06Practice.Features.Account.Store;

public record LoadAccountAction;

public record LoadAccountSuccessAction(
    int OrderCount,
    int WishlistCount,
    string MembershipTier);

public record LoadAccountFailureAction(string ErrorMessage);
