using Fluxor;

namespace Day05Practice.Features.Todo.Store;

public class TodoFeature : Feature<TodoState>
{
    public override string GetName() => "Todo";

    protected override TodoState GetInitialState()
        => new() { Items = [], NextId = 1, IsLoading = false, ErrorMessage = null };
}
