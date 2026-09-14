using Day04Demo.Features.Counter.Store;

namespace Day04Demo.Tests;

public class CounterReducerTests
{
    [Fact]
    public void ReduceIncrementCounterAction_WithCountOf5_ReturnsCountOf6()
    {
        // Arrange
        var state = new CounterState { Count = 5 };
        var action = new IncrementCounterAction();

        // Act
        var result = CounterReducers.ReduceIncrementCounterAction(state, action);

        // Assert
        Assert.Equal(6, result.Count);
    }

    [Fact]
    public void ReduceDecrementCounterAction_WithCountOf3_ReturnsCountOf2()
    {
        // Arrange
        var state = new CounterState { Count = 3 };
        var action = new DecrementCounterAction();

        // Act
        var result = CounterReducers.ReduceDecrementCounterAction(state, action);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void ReduceSetCounterAction_WithNewCountOf42_ReturnsCountOf42()
    {
        // Arrange
        var state = new CounterState { Count = 10 };
        var action = new SetCounterAction(42);

        // Act
        var result = CounterReducers.ReduceSetCounterAction(state, action);

        // Assert
        Assert.Equal(42, result.Count);
    }
}
