namespace Exercises.RxNetCreation;

public class LogStreamTests : ReactiveTest
{
    private static readonly long Second = TimeSpan.TicksPerSecond;

    [Fact]
    public void Clicks_emits_every_click_and_Dispose_removes_the_handler()
    {
        var button = new Button();
        var clicks = new List<string>();
        var subscription = LogStream.Clicks(button).Subscribe(clicks.Add);

        Assert.Equal(1, button.HandlerCount);
        button.Click();
        button.Click();
        subscription.Dispose();
        button.Click();

        Assert.Equal(["click", "click"], clicks);
        Assert.Equal(0, button.HandlerCount);
    }

    [Fact]
    public void Ticks_counts_up_on_schedule()
    {
        var scheduler = new TestScheduler();
        var observer = scheduler.Start(
            () => LogStream.Ticks(TimeSpan.FromSeconds(1), scheduler).Take(3),
            created: 0, subscribed: 0, disposed: 10 * Second);

        // The TestScheduler starts the Interval one tick after Subscribe, hence + 1.
        Assert.Equal(
            [OnNext(1 * Second + 1, 0L), OnNext(2 * Second + 1, 1L), OnNext(3 * Second + 1, 2L), OnCompleted<long>(3 * Second + 1)],
            observer.Messages);
    }

    [Fact]
    public void LoadCustomer_loads_only_on_Subscribe_and_anew_for_every_subscriber()
    {
        var scheduler = new TestScheduler();
        var calls = 0;
        var customer = LogStream.LoadCustomer(
            _ => Task.FromResult(new Customer($"c{++calls}", "Ada")), scheduler);

        Assert.Equal(0, calls);
        var received = new List<Customer>();
        using var first = customer.Subscribe(received.Add);
        using var second = customer.Subscribe(received.Add);
        scheduler.AdvanceBy(1);

        Assert.Equal(2, calls);
        Assert.Equal([new Customer("c1", "Ada"), new Customer("c2", "Ada")], received);
    }

    [Fact]
    public void WithPrefix_puts_the_prefix_before_every_value()
    {
        var lines = new List<string>();
        using var subscription = Observable.Range(1, 2).WithPrefix("tick").Subscribe(lines.Add);
        Assert.Equal(["tick: 1", "tick: 2"], lines);
    }

    [Fact]
    public void Create_merges_clicks_ticks_and_customers_into_one_stream()
    {
        var scheduler = new TestScheduler();
        var button = new Button();
        var lines = new List<string>();
        using var subscription = LogStream
            .Create(button, TimeSpan.FromSeconds(1), _ => Task.FromResult(new Customer("c42", "Ada")), scheduler)
            .Subscribe(lines.Add);

        button.Click();
        scheduler.AdvanceBy(2 * Second);

        Assert.Equal(["click: Klick", "customer: Ada", "tick: 0", "tick: 1"], lines);
    }
}
