namespace Day05Demo.Features.Product.Store;

// The store slice. Init-only properties mean nothing mutates it in place — a reducer
// produces a NEW state via `with`. The three fields map one-to-one to the three things
// the page shows: Items (the catalog), IsLoading (spinner), ErrorMessage (error alert).
// Modelling loading and error AS STATE — not as ad-hoc component fields — is the point of
// the day: the load's whole lifecycle is visible in one immutable record.
public record ProductState
{
    public List<Services.Product> Items { get; init; } = [];
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
}
