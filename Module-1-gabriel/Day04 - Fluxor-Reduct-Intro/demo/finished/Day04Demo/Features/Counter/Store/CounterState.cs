namespace Day04Demo.Features.Counter.Store;

// The store slice for this feature — one immutable record. The init-only Count means nothing
// mutates it in place; a reducer produces a NEW state via `with`. This is the single source of
// truth the Counter component reads from.
public record CounterState
{
    public int Count { get; init; }
}
