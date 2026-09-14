namespace Day04Demo.Features.Counter.Store;

public record IncrementCounterAction;
public record SetCounterAction(int NewCount);
public record DecrementCounterAction;