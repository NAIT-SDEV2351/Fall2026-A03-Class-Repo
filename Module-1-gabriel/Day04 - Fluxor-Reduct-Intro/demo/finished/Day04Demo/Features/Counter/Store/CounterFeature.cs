namespace Day04Demo.Features.Counter.Store;

using Fluxor;

public class CounterFeature : Feature<CounterState>
{
    // GetName() identifies this slice in the store (and Redux DevTools). GetInitialState() is the
    // state before any action runs — Count = 0 is exactly what the Counter shows on first paint.
    public override string GetName() => "Counter";

    protected override CounterState GetInitialState()
    {
        return new CounterState { Count = 0 };
    }
}
