using Day04Practice.Features.Counter.Store;

namespace Day04Practice.Tests;

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

    // TODO: Write a test for DecrementCounterAction
    // Pattern: Arrange state with a known Count, Act with the reducer, Assert the new Count

    // TODO: Write a test for ResetCounterAction
    // Pattern: Arrange state with a non-zero Count, Act with the reducer, Assert Count is 0
}
