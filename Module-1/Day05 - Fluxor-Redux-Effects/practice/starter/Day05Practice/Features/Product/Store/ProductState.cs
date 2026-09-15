using Day05Practice.Services;

namespace Day05Practice.Features.Product.Store;

// Practice 2: Define the ProductState record
// TODO: Add three properties:
//   - List<Services.Product> Items (default: empty list)
//     NOTE: the Services. qualifier is REQUIRED. Inside this namespace
//     (Day05Practice.Features.Product.Store), the bare name "Product" binds to the
//     NAMESPACE Day05Practice.Features.Product, not to the type -- and the using
//     directive on line 1 cannot override that. Same reason as the demonstration.
//   - bool IsLoading (default: false)
//   - string? ErrorMessage (default: null)

public record ProductState
{
    // TODO: Add properties here

    // TODO (Practice 2, items 2-5): Create ProductActions.cs, ProductReducers.cs, ProductEffects.cs
    // and ProductFeature.cs in this folder — see the practice walkthrough.
}
