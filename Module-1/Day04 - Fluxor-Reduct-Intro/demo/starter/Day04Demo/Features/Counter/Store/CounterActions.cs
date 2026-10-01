namespace Day04Demo.Features.Counter.Store;

public record IncrementCounterAction;

public record DecrementCounterAction;

public record SetCounterAction(int NewCount);