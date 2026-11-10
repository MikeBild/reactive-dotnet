// Lesson rxnet-ui-state: UI state as a stream of actions
//
// Task: Build a ViewModel with actions as a stream, manage the state with Scan through
// pure Reducers, and keep the last five searches as an LRU list.
//
// Actions in, state out. The ViewModel knows no UI framework and runs in the test
// without a window.

namespace Exercises.RxNetUiState;

public static class Reducers
{
    /// <summary>Pure Reducer for the cart. Always returns a new list.</summary>
    public static IReadOnlyList<CartItem> ReduceCart(IReadOnlyList<CartItem> cart, CartAction action)
    {
        // TODO: switch on action:
        //  - AddItem: if the product already exists, increase Quantity (with "with"), otherwise append it
        //  - RemoveItem: remove the line item
        throw new NotImplementedException("TODO: ReduceCart");
    }

    /// <summary>Pure Reducer for the recent searches: newest first, no duplicates, at most limit.</summary>
    public static IReadOnlyList<string> PushRecent(IReadOnlyList<string> recent, string term, int limit = 5)
    {
        // TODO: ignore empty terms, remove the duplicate, insert at the front, trim to limit
        throw new NotImplementedException("TODO: PushRecent");
    }
}

public sealed class CartViewModel
{
    // TODO: Subject<CartAction> for actions, Subject<string> for searches,
    //       BehaviorSubject<bool> for “order in progress”

    public CartViewModel()
    {
        // TODO: Cart with Scan(Reducers.ReduceCart), StartWith, and Replay(1).RefCount()
        //       Careful: StartWith([]) binds to params T[] and yields no initial value at all,
        //       write StartWith((IReadOnlyList<CartItem>)[]).
        //       TotalCents from Cart with Select(Shop.Cart.TotalCents) and DistinctUntilChanged
        //       CanPlace with CombineLatest of Cart and Placing
        //       RecentSearches with Scan(Reducers.PushRecent) and StartWith
        throw new NotImplementedException("TODO: CartViewModel");
    }

    public IObservable<IReadOnlyList<CartItem>> Cart { get; } = Observable.Never<IReadOnlyList<CartItem>>();
    public IObservable<int> TotalCents { get; } = Observable.Never<int>();
    public IObservable<bool> CanPlace { get; } = Observable.Never<bool>();
    public IObservable<IReadOnlyList<string>> RecentSearches { get; } = Observable.Never<IReadOnlyList<string>>();

    public void Add(Product product, int quantity = 1) => throw new NotImplementedException("TODO: Add");
    public void Remove(string productId) => throw new NotImplementedException("TODO: Remove");
    public void SetPlacing(bool placing) => throw new NotImplementedException("TODO: SetPlacing");
    public void Search(string term) => throw new NotImplementedException("TODO: Search");
}
