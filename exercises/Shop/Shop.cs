namespace Exercises.Shop;

// Der Werkstatt-Shop, das durchgehende Beispiel aller Workshops.
// Geld immer als ganze Cent, nie als Kommazahl.

public sealed record Product(string Id, string Name, int PriceCents, int Stock);

public sealed record CartItem(string ProductId, int Quantity, int PriceCents);

public sealed record Customer(string Id, string Name);

public enum OrderStatus { Draft, Placed, Paid, Shipped, Cancelled }

public static class Cart
{
    /// <summary>Gesamtsumme in Cent: Summe aus Quantity mal PriceCents.</summary>
    public static int TotalCents(IEnumerable<CartItem> items) =>
        items.Sum(item => item.Quantity * item.PriceCents);
}
