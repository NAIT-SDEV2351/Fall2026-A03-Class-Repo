namespace Day05Practice.Features.Product.Store;

// Trigger -- dispatched by the component to start loading
public record LoadProductsAction;

// Success -- dispatched by the effect when data arrives
public record LoadProductsSuccessAction(List<Services.Product> Products);

// Failure -- dispatched by the effect when the service call fails
public record LoadProductsFailureAction(string Error);
