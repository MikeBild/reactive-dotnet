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
    public void OfMessageType_lets_only_the_requested_type_through_and_yields_the_payload()
    {
        var statuses = Collect(new[] { Placed, Chat, Paid }.ToObservable().OfMessageType<OrderStatusChanged>("order-status"));

        Assert.Equal([OrderStatus.Placed, OrderStatus.Paid], statuses.Select(s => s.Status));
    }

    [Fact]
    public void OfMessageType_drops_directly_repeated_ids()
    {
        var statuses = Collect(new[] { Placed, Placed, Paid, Paid, Placed }.ToObservable()
            .OfMessageType<OrderStatusChanged>("order-status"));

        Assert.Equal([OrderStatus.Placed, OrderStatus.Paid, OrderStatus.Placed], statuses.Select(s => s.Status));
    }

    [Fact]
    public void CountOfType_emits_the_running_count_after_every_message()
    {
        var messages = new Subject<Message>();
        var counts = new List<int>();
        using var subscription = messages.CountOfType("order-status").Subscribe(counts.Add);

        messages.OnNext(Placed);
        messages.OnNext(Placed); // duplicate
        messages.OnNext(Chat);
        messages.OnNext(Paid);

        // The stream has not completed, yet the intermediate counts are already there.
        Assert.Equal([1, 2], counts);
    }
}
