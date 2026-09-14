namespace Day03Demo.Models;

// A record: immutable cart line. Changing quantity means copying with `with` (see
// Products.AddToCart), not mutating in place — the copy-don't-mutate idiom students reuse
// for Fluxor state from Day 4 on.
public record CartItem(int ProductId, string Name, decimal Price, int Quantity);
