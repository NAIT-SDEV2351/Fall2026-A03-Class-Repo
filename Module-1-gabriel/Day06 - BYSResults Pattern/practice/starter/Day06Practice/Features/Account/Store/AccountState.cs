namespace Day06Practice.Features.Account.Store;

public record AccountState
{
    public int OrderCount { get; init; }
    public int WishlistCount { get; init; }
    public string MembershipTier { get; init; } = string.Empty;
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
}
