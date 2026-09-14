using Day04Practice.Features.Counter.Store;

namespace Day04Practice.Tests;

public class CounterReducerTests
{
    [Fact]
    public void ReduceIncrementCounterAction_WithCountOf5_ReturnsCountOf6()
    {
        var state = new CounterState { Count = 5 };
        var action = new IncrementCounterAction();

        var result = CounterReducers.ReduceIncrementCounterAction(state, action);

        Assert.Equal(6, result.Count);
    }

    [Fact]
    public void ReduceDecrementCounterAction_WithCountOf3_ReturnsCountOf2()
    {
        var state = new CounterState { Count = 3 };
        var action = new DecrementCounterAction();

        var result = CounterReducers.ReduceDecrementCounterAction(state, action);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void ReduceResetCounterAction_WithCountOf10_ReturnsCountOf0()
    {
        var state = new CounterState { Count = 10 };
        var action = new ResetCounterAction();

        var result = CounterReducers.ReduceResetCounterAction(state, action);

        Assert.Equal(0, result.Count);
    }
}
