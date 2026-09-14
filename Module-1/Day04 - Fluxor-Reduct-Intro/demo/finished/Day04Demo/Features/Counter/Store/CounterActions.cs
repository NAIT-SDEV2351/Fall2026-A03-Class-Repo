namespace Day04Demo.Features.Counter.Store;

// Actions are immutable record "messages" describing something that should happen — they carry
// no logic. Increment/Decrement are parameterless (they need no data); SetCounterAction carries
// a payload (NewCount) the reducer copies into state. Dispatching one is how the UI asks the
// store to change — the component never edits state itself.
public record IncrementCounterAction;
public record DecrementCounterAction;
public record SetCounterAction(int NewCount);
