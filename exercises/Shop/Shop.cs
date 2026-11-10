namespace Exercises.Shop;

// The workshop shop, the running example in all workshops.
// Money always as whole cents, never as a decimal number.

public sealed record Product(string Id, string Name, int PriceCents, int Stock);

public sealed record CartItem(string ProductId, int Quantity, int PriceCents);

public sealed record Customer(string Id, string Name);

public enum OrderStatus { Draft, Placed, Paid, Shipped, Cancelled }

public static class Cart
{
    /// <summary>Total in cents: sum of Quantity times PriceCents.</summary>
    public static int TotalCents(IEnumerable<CartItem> items) =>
        items.Sum(item => item.Quantity * item.PriceCents);
}
