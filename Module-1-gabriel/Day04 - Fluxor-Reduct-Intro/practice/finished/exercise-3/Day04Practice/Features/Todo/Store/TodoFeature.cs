namespace Day04Practice.Features.Todo.Store;

using Fluxor;

public class TodoFeature : Feature<TodoState>
{
    public override string GetName() => "Todo";

    protected override TodoState GetInitialState()
    {
        return new TodoState();
    }
}
