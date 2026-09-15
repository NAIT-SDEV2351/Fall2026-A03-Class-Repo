namespace Day05Demo.Features.Product.Store;

// The request / success / failure action triad — the shape every async load follows.
// The component dispatches the request (LoadProductsAction); the effect dispatches
// exactly ONE of the other two when the service call finishes. Records are immutable
// messages — the constructor parameters are the payload the reducers copy into state.

// Trigger — dispatched by the component to start loading (no payload; it just says "go").
public record LoadProductsAction;

// Success — the effect dispatches this when data arrives; the list IS the payload.
public record LoadProductsSuccessAction(List<Services.Product> Products);

// Failure — the effect dispatches this when the service call throws; carries the message.
public record LoadProductsFailureAction(string Error);
