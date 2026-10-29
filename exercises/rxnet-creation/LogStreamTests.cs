namespace Exercises.RxNetCreation;

public class LogStreamTests : ReactiveTest
{
    private static readonly long Second = TimeSpan.TicksPerSecond;

    [Fact]
    public void Clicks_liefert_jeden_Klick_und_Dispose_meldet_den_Handler_ab()
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
    public void Ticks_zaehlt_im_Takt_hoch()
    {
        var scheduler = new TestScheduler();
        var observer = scheduler.Start(
            () => LogStream.Ticks(TimeSpan.FromSeconds(1), scheduler).Take(3),
            created: 0, subscribed: 0, disposed: 10 * Second);

        // Der TestScheduler startet das Interval einen Tick nach dem Subscribe, daher + 1.
        Assert.Equal(
            [OnNext(1 * Second + 1, 0L), OnNext(2 * Second + 1, 1L), OnNext(3 * Second + 1, 2L), OnCompleted<long>(3 * Second + 1)],
            observer.Messages);
    }

    [Fact]
    public void LoadCustomer_laedt_erst_beim_Subscribe_fuer_jeden_Subscriber_neu()
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
    public void WithPrefix_setzt_den_Praefix_vor_jeden_Wert()
    {
        var lines = new List<string>();
        using var subscription = Observable.Range(1, 2).WithPrefix("tick").Subscribe(lines.Add);
        Assert.Equal(["tick: 1", "tick: 2"], lines);
    }

    [Fact]
    public void Create_fuehrt_Klicks_Ticks_und_Kunden_in_einem_Stream_zusammen()
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
