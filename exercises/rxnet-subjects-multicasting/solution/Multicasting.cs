namespace Exercises.RxNetSubjectsMulticasting;

public static class Socket
{
    /// <summary>Cold: Jeder Subscriber öffnet eine eigene Verbindung, Dispose schließt sie.</summary>
    public static IObservable<string> Connect(FakeSocketServer server) =>
        Observable.Create<string>(observer =>
        {
            var connection = server.Connect();
            connection.OnMessage(observer.OnNext);
            return Disposable.Create(connection.Close);
        });

    /// <summary>Hot: eine geteilte Verbindung für alle Subscriber.</summary>
    public static IObservable<string> Shared(FakeSocketServer server) =>
        // Publish verteilt eine Verbindung an alle, RefCount zählt die Subscriber mit:
        // Der erste öffnet die Verbindung, der letzte Dispose schließt sie wieder.
        // Wie im Sample SharedResource: Der Letzte macht das Licht aus.
        Connect(server).Publish().RefCount();
}

/// <summary>Der Warenkorb als Zustand mit aktuellem Wert.</summary>
public sealed class CartStore
{
    // BehaviorSubject statt Subject: Es hat einen Startwert und gibt jedem neuen
    // Subscriber sofort den aktuellen Stand.
    private readonly BehaviorSubject<IReadOnlyList<CartItem>> _items = new([]);

    /// <summary>Nach außen nur die Observable-Seite, nie das Subject selbst.</summary>
    public IObservable<IReadOnlyList<CartItem>> Items => _items.AsObservable();

    /// <summary>Der aktuelle Stand, synchron lesbar.</summary>
    public IReadOnlyList<CartItem> Current => _items.Value;

    public void Add(CartItem item) => _items.OnNext([.. _items.Value, item]);
}
