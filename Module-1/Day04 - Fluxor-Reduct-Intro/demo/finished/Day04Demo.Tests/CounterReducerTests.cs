using Day04Demo.Features.Counter.Store;

namespace Day04Demo.Tests;

// Why reducers are easy to test: because they're PURE functions, a test just builds a state,
// calls the reducer as a plain static method, and asserts on the returned state — no store, no
// mocks, no async, no Blazor. Purity is what buys this testability.
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
    public void ReduceSetCounterAction_WithNewCountOf42_ReturnsCountOf42()
    {
        var state = new CounterState { Count = 10 };
        var action = new SetCounterAction(42);

        var result = CounterReducers.ReduceSetCounterAction(state, action);

        Assert.Equal(42, result.Count);
    }
}
