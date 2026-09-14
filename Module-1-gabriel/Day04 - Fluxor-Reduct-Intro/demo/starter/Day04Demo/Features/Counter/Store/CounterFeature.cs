namespace Day04Demo.Features.Counter.Store;

using Fluxor;

public class CounterFeature : Feature<CounterState>
{
    public override string GetName() => "Counter";

    protected override CounterState GetInitialState()
    {
        return new CounterState { Count = 0 };
    }
}