// Lektion rxnet-subjects-multicasting: Subjects teilen Werte, Publish teilt Arbeit
//
// Aufgabe: Teile eine simulierte WebSocket-Verbindung zwischen mehreren Subscribern,
// sodass sie nur einmal öffnet und beim letzten Dispose schließt.
//
// Der Letzte macht das Licht aus. Dazu ein CartStore mit BehaviorSubject.

namespace Exercises.RxNetSubjectsMulticasting;

public static class Socket
{
    /// <summary>Cold: Jeder Subscriber öffnet eine eigene Verbindung, Dispose schließt sie.</summary>
    public static IObservable<string> Connect(FakeSocketServer server)
    {
        // TODO: Observable.Create: server.Connect(), connection.OnMessage(observer.OnNext),
        //       als Teardown connection.Close() zurückgeben (Disposable.Create)
        throw new NotImplementedException("TODO: Connect");
    }

    /// <summary>Hot: eine geteilte Verbindung für alle Subscriber.</summary>
    public static IObservable<string> Shared(FakeSocketServer server)
    {
        // TODO: Connect mit Publish().RefCount() teilen
        throw new NotImplementedException("TODO: Shared");
    }
}

/// <summary>Der Warenkorb als Zustand mit aktuellem Wert.</summary>
public sealed class CartStore
{
    // TODO: ein BehaviorSubject<IReadOnlyList<CartItem>> mit leerer Liste als Startwert

    /// <summary>Nach außen nur die Observable-Seite, nie das Subject selbst.</summary>
    public IObservable<IReadOnlyList<CartItem>> Items =>
        throw new NotImplementedException("TODO: Items mit AsObservable");

    /// <summary>Der aktuelle Stand, synchron lesbar.</summary>
    public IReadOnlyList<CartItem> Current =>
        throw new NotImplementedException("TODO: Current über Value");

    public void Add(CartItem item) =>
        throw new NotImplementedException("TODO: Add mit OnNext und einer neuen Liste");
}
