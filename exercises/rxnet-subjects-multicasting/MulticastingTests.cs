namespace Exercises.RxNetSubjectsMulticasting;

public class MulticastingTests
{
    private static readonly CartItem Brake = new("p1", 1, 4_990);
    private static readonly CartItem Plug = new("p2", 4, 790);

    [Fact]
    public void Connect_opens_a_separate_connection_per_subscriber()
    {
        var server = new FakeSocketServer();
        var socket = Socket.Connect(server);

        var a = socket.Subscribe(_ => { });
        var b = socket.Subscribe(_ => { });
        Assert.Equal(2, server.Opened);

        a.Dispose();
        b.Dispose();
        Assert.Equal(0, server.OpenConnections);
    }

    [Fact]
    public void Shared_opens_exactly_one_connection_for_all()
    {
        var server = new FakeSocketServer();
        var socket = Socket.Shared(server);
        var first = new List<string>();
        var second = new List<string>();

        using var a = socket.Subscribe(first.Add);
        using var b = socket.Subscribe(second.Add);
        server.Broadcast("order o1 placed");

        Assert.Equal(1, server.Opened);
        Assert.Equal(["order o1 placed"], first);
        Assert.Equal(["order o1 placed"], second);
    }

    [Fact]
    public void Shared_the_last_one_turns_off_the_lights()
    {
        var server = new FakeSocketServer();
        var socket = Socket.Shared(server);

        var a = socket.Subscribe(_ => { });
        var b = socket.Subscribe(_ => { });
        a.Dispose();
        Assert.Equal(1, server.OpenConnections); // b is still listening

        b.Dispose();
        Assert.Equal(0, server.OpenConnections);
        Assert.Equal(1, server.Closed);
    }

    [Fact]
    public void Shared_reopens_on_the_next_Subscribe_after_the_last_Dispose()
    {
        var server = new FakeSocketServer();
        var socket = Socket.Shared(server);

        socket.Subscribe(_ => { }).Dispose();
        using var again = socket.Subscribe(_ => { });

        Assert.Equal(2, server.Opened);
        Assert.Equal(1, server.OpenConnections);
    }

    [Fact]
    public void CartStore_gives_new_subscribers_the_current_state_immediately()
    {
        var store = new CartStore();
        store.Add(Brake);

        var seen = new List<IReadOnlyList<CartItem>>();
        using var subscription = store.Items.Subscribe(seen.Add);
        store.Add(Plug);

        Assert.Equal(2, seen.Count);
        Assert.Equal([Brake], seen[0]);
        Assert.Equal([Brake, Plug], seen[1]);
        Assert.Equal([Brake, Plug], store.Current);
    }

    [Fact]
    public void CartStore_does_not_expose_the_Subject() =>
        Assert.IsNotType<BehaviorSubject<IReadOnlyList<CartItem>>>(new CartStore().Items);
}
