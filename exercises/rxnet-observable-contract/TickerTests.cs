namespace Exercises.RxNetObservableContract;

public class TickerTests : ReactiveTest
{
    private static readonly TimeSpan Second = TimeSpan.FromSeconds(1);

    /// <summary>TestScheduler that tells whether work is still scheduled, i.e. whether a timer is running.</summary>
    private sealed class InspectableScheduler : TestScheduler
    {
        public bool HasPendingWork => GetNext() is not null;
    }

    [Fact]
    public void Without_Subscribe_no_timer_starts()
    {
        var scheduler = new InspectableScheduler();
        Ticker.Create(Second, scheduler);
        Assert.False(scheduler.HasPendingWork);
    }

    [Fact]
    public void Emits_five_values_on_schedule_and_then_signals_OnCompleted()
    {
        var scheduler = new InspectableScheduler();
        var values = new List<int>();
        var completed = false;
        using var subscription = Ticker.Create(Second, scheduler).Subscribe(values.Add, () => completed = true);

        scheduler.AdvanceBy(2 * Second.Ticks);
        Assert.Equal([0, 1], values);
        Assert.False(completed);

        scheduler.AdvanceBy(3 * Second.Ticks);
        Assert.Equal([0, 1, 2, 3, 4], values);
        Assert.True(completed);
    }

    [Fact]
    public void After_OnCompleted_no_timer_is_running()
    {
        var scheduler = new InspectableScheduler();
        using var subscription = Ticker.Create(Second, scheduler).Subscribe(_ => { });

        scheduler.AdvanceBy(10 * Second.Ticks);
        Assert.False(scheduler.HasPendingWork);
    }

    [Fact]
    public void Dispose_stops_the_timer()
    {
        var scheduler = new InspectableScheduler();
        var values = new List<int>();
        var subscription = Ticker.Create(Second, scheduler).Subscribe(values.Add);

        scheduler.AdvanceBy(2 * Second.Ticks + Second.Ticks / 2);
        subscription.Dispose();
        scheduler.AdvanceBy(10 * Second.Ticks);

        Assert.Equal([0, 1], values);
        Assert.False(scheduler.HasPendingWork);
    }

    [Fact]
    public void Every_subscriber_gets_its_own_timer()
    {
        var scheduler = new InspectableScheduler();
        var ticks = Ticker.Create(Second, scheduler);
        var first = new List<int>();
        var second = new List<int>();

        using var a = ticks.Subscribe(first.Add);
        scheduler.AdvanceBy(2 * Second.Ticks);
        using var b = ticks.Subscribe(second.Add);
        scheduler.AdvanceBy(2 * Second.Ticks);

        Assert.Equal([0, 1, 2, 3], first);
        Assert.Equal([0, 1], second); // the second timer starts at 0
    }
}
