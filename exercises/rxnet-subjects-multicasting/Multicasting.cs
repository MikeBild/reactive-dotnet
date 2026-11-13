// Lesson rxnet-subjects-multicasting: Subjects share values, Publish shares work
//
// Task: Share a simulated WebSocket connection between several subscribers
// so that it opens only once and closes on the last Dispose.
//
// The last one turns off the light. Plus a CartStore with BehaviorSubject.

namespace Exercises.RxNetSubjectsMulticasting;

public static class Socket
{
    /// <summary>Cold: Every subscriber opens its own connection, Dispose closes it.</summary>
    public static IObservable<string> Connect(FakeSocketServer server)
    {
        // TODO: Observable.Create: server.Connect(), connection.OnMessage(observer.OnNext),
        //       return connection.Close() as the teardown (Disposable.Create)
        throw new NotImplementedException("TODO: Connect");
    }

    /// <summary>Hot: one shared connection for all subscribers.</summary>
    public static IObservable<string> Shared(FakeSocketServer server)
    {
        // TODO: Share Connect with Publish().RefCount()
        throw new NotImplementedException("TODO: Shared");
    }
}

/// <summary>The cart as state with a current value.</summary>
public sealed class CartStore
{
    // TODO: a BehaviorSubject<IReadOnlyList<CartItem>> with an empty list as the initial value

    /// <summary>Expose only the Observable side, never the Subject itself.</summary>
    public IObservable<IReadOnlyList<CartItem>> Items =>
        throw new NotImplementedException("TODO: Items with AsObservable");

    /// <summary>The current state, readable synchronously.</summary>
    public IReadOnlyList<CartItem> Current =>
        throw new NotImplementedException("TODO: Current via Value");

    public void Add(CartItem item) =>
        throw new NotImplementedException("TODO: Add with OnNext and a new list");
}
