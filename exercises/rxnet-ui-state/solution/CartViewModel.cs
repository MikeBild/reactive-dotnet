namespace Exercises.RxNetUiState;

public static class Reducers
{
    /// <summary>Pure Reducer for the cart. Always returns a new list.</summary>
    public static IReadOnlyList<CartItem> ReduceCart(IReadOnlyList<CartItem> cart, CartAction action) => action switch
    {
        // Only the changed line item is created anew, all others stay the same objects.
        AddItem { Item: var item } when cart.Any(entry => entry.ProductId == item.ProductId) =>
            [.. cart.Select(entry => entry.ProductId == item.ProductId
                ? entry with { Quantity = entry.Quantity + item.Quantity }
                : entry)],
        AddItem { Item: var item } => [.. cart, item],
        RemoveItem { ProductId: var productId } => [.. cart.Where(entry => entry.ProductId != productId)],
        _ => cart,
    };

    /// <summary>Pure Reducer for the recent searches: newest first, no duplicates, at most limit.</summary>
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
        // Actions in, state out. Replay(1).RefCount() matches shareReplay with refCount in RxJS:
        // Late subscribers get the current state right away.
        Cart = _actions
            .Scan((IReadOnlyList<CartItem>)[], Reducers.ReduceCart)
            // Careful: StartWith([]) would be the params call with zero initial values.
            // The empty list must be passed as one value of type IReadOnlyList.
            .StartWith((IReadOnlyList<CartItem>)[])
            .Replay(1)
            .RefCount();

        // Derived state instead of flags that you have to keep in sync by hand.
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

    // Expose only methods, never the Subjects themselves.
    public void Add(Product product, int quantity = 1) =>
        _actions.OnNext(new AddItem(new CartItem(product.Id, quantity, product.PriceCents)));

    public void Remove(string productId) => _actions.OnNext(new RemoveItem(productId));

    public void SetPlacing(bool placing) => _placing.OnNext(placing);

    public void Search(string term) => _searches.OnNext(term);
}
