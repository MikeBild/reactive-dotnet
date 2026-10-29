// Lektion rxnet-ui-state: UI-Zustand als Stream von Aktionen
//
// Aufgabe: Bau ein ViewModel mit Aktionen als Stream, führe den Zustand mit Scan über
// reine Reducer und halte die letzten fünf Suchen als LRU-Liste.
//
// Aktionen rein, Zustand raus. Das ViewModel kennt kein UI-Framework und läuft im Test
// ohne Fenster.

namespace Exercises.RxNetUiState;

public static class Reducers
{
    /// <summary>Reiner Reducer für den Warenkorb. Gibt immer eine neue Liste zurück.</summary>
    public static IReadOnlyList<CartItem> ReduceCart(IReadOnlyList<CartItem> cart, CartAction action)
    {
        // TODO: switch über action:
        //  - AddItem: gibt es das Produkt schon, Quantity erhöhen (mit "with"), sonst anhängen
        //  - RemoveItem: Position entfernen
        throw new NotImplementedException("TODO: ReduceCart");
    }

    /// <summary>Reiner Reducer für die letzten Suchen: neueste vorn, ohne Duplikate, höchstens limit.</summary>
    public static IReadOnlyList<string> PushRecent(IReadOnlyList<string> recent, string term, int limit = 5)
    {
        // TODO: leere Begriffe ignorieren, Duplikat entfernen, vorn einfügen, auf limit kürzen
        throw new NotImplementedException("TODO: PushRecent");
    }
}

public sealed class CartViewModel
{
    // TODO: Subject<CartAction> für Aktionen, Subject<string> für Suchen,
    //       BehaviorSubject<bool> für „Bestellung läuft“

    public CartViewModel()
    {
        // TODO: Cart mit Scan(Reducers.ReduceCart), StartWith und Replay(1).RefCount()
        //       Vorsicht: StartWith([]) bindet an params T[] und liefert gar keinen Startwert,
        //       schreib StartWith((IReadOnlyList<CartItem>)[]).
        //       TotalCents aus Cart mit Select(Shop.Cart.TotalCents) und DistinctUntilChanged
        //       CanPlace mit CombineLatest aus Cart und Placing
        //       RecentSearches mit Scan(Reducers.PushRecent) und StartWith
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
