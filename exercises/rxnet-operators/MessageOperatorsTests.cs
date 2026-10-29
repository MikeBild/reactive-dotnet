namespace Exercises.RxNetOperators;

public class MessageOperatorsTests
{
    private static readonly Message Placed = new("order-status", "m1", new OrderStatusChanged("o1", OrderStatus.Placed));
    private static readonly Message Paid = new("order-status", "m2", new OrderStatusChanged("o1", OrderStatus.Paid));
    private static readonly Message Chat = new("chat", "m3", "Hallo");

    private static List<T> Collect<T>(IObservable<T> source)
    {
        var values = new List<T>();
        using var subscription = source.Subscribe(values.Add);
        return values;
    }

    [Fact]
    public void OfMessageType_laesst_nur_den_gewuenschten_Typ_durch_und_liefert_den_Payload()
    {
        var statuses = Collect(new[] { Placed, Chat, Paid }.ToObservable().OfMessageType<OrderStatusChanged>("order-status"));

        Assert.Equal([OrderStatus.Placed, OrderStatus.Paid], statuses.Select(s => s.Status));
    }

    [Fact]
    public void OfMessageType_laesst_direkt_wiederholte_Ids_weg()
    {
        var statuses = Collect(new[] { Placed, Placed, Paid, Paid, Placed }.ToObservable()
            .OfMessageType<OrderStatusChanged>("order-status"));

        Assert.Equal([OrderStatus.Placed, OrderStatus.Paid, OrderStatus.Placed], statuses.Select(s => s.Status));
    }

    [Fact]
    public void CountOfType_liefert_nach_jeder_Nachricht_den_Zwischenstand()
    {
        var messages = new Subject<Message>();
        var counts = new List<int>();
        using var subscription = messages.CountOfType("order-status").Subscribe(counts.Add);

        messages.OnNext(Placed);
        messages.OnNext(Placed); // Duplikat
        messages.OnNext(Chat);
        messages.OnNext(Paid);

        // Der Strom ist nicht beendet, trotzdem liegen die Zwischenstände schon vor.
        Assert.Equal([1, 2], counts);
    }
}
