namespace Exercises.RxNetSubjectsMulticasting;

public static class Socket
{
    /// <summary>Cold: Every subscriber opens its own connection, Dispose closes it.</summary>
    public static IObservable<string> Connect(FakeSocketServer server) =>
        Observable.Create<string>(observer =>
        {
            var connection = server.Connect();
            connection.OnMessage(observer.OnNext);
            return Disposable.Create(connection.Close);
        });

    /// <summary>Hot: one shared connection for all subscribers.</summary>
    public static IObservable<string> Shared(FakeSocketServer server) =>
        // Publish distributes one connection to all, RefCount keeps count of the subscribers:
        // The first one opens the connection, the last Dispose closes it again.
        // As in the SharedResource sample: The last one turns off the light.
        Connect(server).Publish().RefCount();
}

/// <summary>The cart as state with a current value.</summary>
public sealed class CartStore
{
    // BehaviorSubject instead of Subject: It has an initial value and gives every new
    // subscriber the current state right away.
    private readonly BehaviorSubject<IReadOnlyList<CartItem>> _items = new([]);

    /// <summary>Expose only the Observable side, never the Subject itself.</summary>
    public IObservable<IReadOnlyList<CartItem>> Items => _items.AsObservable();

    /// <summary>The current state, readable synchronously.</summary>
    public IReadOnlyList<CartItem> Current => _items.Value;

    public void Add(CartItem item) => _items.OnNext([.. _items.Value, item]);
}
