namespace Exercises.RxNetSubjectsMulticasting;

public class MulticastingTests
{
    private static readonly CartItem Brake = new("p1", 1, 4_990);
    private static readonly CartItem Plug = new("p2", 4, 790);

    [Fact]
    public void Connect_oeffnet_pro_Subscriber_eine_eigene_Verbindung()
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
    public void Shared_oeffnet_genau_eine_Verbindung_fuer_alle()
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
    public void Shared_der_Letzte_macht_das_Licht_aus()
    {
        var server = new FakeSocketServer();
        var socket = Socket.Shared(server);

        var a = socket.Subscribe(_ => { });
        var b = socket.Subscribe(_ => { });
        a.Dispose();
        Assert.Equal(1, server.OpenConnections); // b hört noch zu

        b.Dispose();
        Assert.Equal(0, server.OpenConnections);
        Assert.Equal(1, server.Closed);
    }

    [Fact]
    public void Shared_oeffnet_nach_dem_letzten_Dispose_beim_naechsten_Subscribe_neu()
    {
        var server = new FakeSocketServer();
        var socket = Socket.Shared(server);

        socket.Subscribe(_ => { }).Dispose();
        using var again = socket.Subscribe(_ => { });

        Assert.Equal(2, server.Opened);
        Assert.Equal(1, server.OpenConnections);
    }

    [Fact]
    public void CartStore_gibt_neuen_Subscribern_sofort_den_aktuellen_Stand()
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
    public void CartStore_gibt_das_Subject_nicht_heraus() =>
        Assert.IsNotType<BehaviorSubject<IReadOnlyList<CartItem>>>(new CartStore().Items);
}
