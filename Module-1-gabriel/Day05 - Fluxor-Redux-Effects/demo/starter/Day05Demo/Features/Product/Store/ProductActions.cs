namespace Day05Demo.Features.Product.Store;

public record LoadProductsAction;

public record LoadProductsSuccessAction(List<Services.Product> Items);

public record LoadProductsFailureAction(string Error);