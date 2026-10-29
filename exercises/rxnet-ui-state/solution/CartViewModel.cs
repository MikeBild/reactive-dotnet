namespace Exercises.RxNetUiState;

public static class Reducers
{
    /// <summary>Reiner Reducer für den Warenkorb. Gibt immer eine neue Liste zurück.</summary>
    public static IReadOnlyList<CartItem> ReduceCart(IReadOnlyList<CartItem> cart, CartAction action) => action switch
    {
        // Nur die geänderte Position wird neu erzeugt, alle anderen bleiben dieselben Objekte.
        AddItem { Item: var item } when cart.Any(entry => entry.ProductId == item.ProductId) =>
            [.. cart.Select(entry => entry.ProductId == item.ProductId
                ? entry with { Quantity = entry.Quantity + item.Quantity }
                : entry)],
        AddItem { Item: var item } => [.. cart, item],
        RemoveItem { ProductId: var productId } => [.. cart.Where(entry => entry.ProductId != productId)],
        _ => cart,
    };

    /// <summary>Reiner Reducer für die letzten Suchen: neueste vorn, ohne Duplikate, höchstens limit.</summary>
    public static IReadOnlyList<string> PushRecent(IReadOnlyList<string> recent, string term, int limit = 5)
    {
        var trimmed = term.Trim();
        if (trimmed.Length == 0) return recent;
        return [.. recent.Where(entry => entry != trimmed).Prepend(trimmed).Take(limit)];
    }
}

public sealed class CartViewModel
{
    private readonly Subject<CartAction> _actions = new();
    private readonly Subject<string> _searches = new();
    private readonly BehaviorSubject<bool> _placing = new(false);

    public CartViewModel()
    {
        // Aktionen rein, Zustand raus. Replay(1).RefCount() entspricht shareReplay mit refCount in RxJS:
        // Späte Subscriber bekommen sofort den aktuellen Stand.
        Cart = _actions
            .Scan((IReadOnlyList<CartItem>)[], Reducers.ReduceCart)
            // Achtung: StartWith([]) wäre der params-Aufruf mit null Startwerten.
            // Die leere Liste muss als ein Wert vom Typ IReadOnlyList übergeben werden.
            .StartWith((IReadOnlyList<CartItem>)[])
            .Replay(1)
            .RefCount();

        // Abgeleiteter Zustand statt Flags, die du von Hand synchron halten musst.
        TotalCents = Cart
            .Select(Shop.Cart.TotalCents)
            .DistinctUntilChanged();

        CanPlace = Cart
            .CombineLatest(_placing, (cart, placing) => cart.Count > 0 && !placing)
            .DistinctUntilChanged();

        RecentSearches = _searches
            .Scan((IReadOnlyList<string>)[], (recent, term) => Reducers.PushRecent(recent, term))
            .DistinctUntilChanged()
            .StartWith((IReadOnlyList<string>)[])
            .Replay(1)
            .RefCount();
    }

    public IObservable<IReadOnlyList<CartItem>> Cart { get; }
    public IObservable<int> TotalCents { get; }
    public IObservable<bool> CanPlace { get; }
    public IObservable<IReadOnlyList<string>> RecentSearches { get; }

    // Nach außen nur Methoden, nie die Subjects selbst.
    public void Add(Product product, int quantity = 1) =>
        _actions.OnNext(new AddItem(new CartItem(product.Id, quantity, product.PriceCents)));

    public void Remove(string productId) => _actions.OnNext(new RemoveItem(productId));

    public void SetPlacing(bool placing) => _placing.OnNext(placing);

    public void Search(string term) => _searches.OnNext(term);
}
