using Fluxor;
using Day06Practice.Services;

namespace Day06Practice.Features.Account.Store;

public class AccountEffects(IAccountService accountService)
{
    [EffectMethod]
    public async Task HandleLoadAccountAction(
        LoadAccountAction action, IDispatcher dispatcher)
    {
        try
        {
            var data = await accountService.GetAccountSummaryAsync();
            dispatcher.Dispatch(new LoadAccountSuccessAction(
                data.OrderCount, data.WishlistCount, data.MembershipTier));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadAccountFailureAction(ex.Message));
        }
    }
}
