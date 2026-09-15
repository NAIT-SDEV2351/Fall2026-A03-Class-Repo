using Fluxor;

namespace Day05Practice.Features.Todo.Store;

public class TodoFeature : Feature<TodoState>
{
    public override string GetName() => "Todo";

    protected override TodoState GetInitialState()
        // TODO (Practice 1, after Task 2): You can optionally add IsLoading = false, ErrorMessage = null
        // here to match the explicit style in the demo's ProductFeature. It is not required — the
        // IsLoading / ErrorMessage properties already on TodoState default to false and null, so the
        // initial state is correct without adding them here.
        => new() { Items = [], NextId = 1 };
}
