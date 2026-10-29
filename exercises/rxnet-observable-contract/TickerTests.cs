namespace Exercises.RxNetObservableContract;

public class TickerTests : ReactiveTest
{
    private static readonly TimeSpan Second = TimeSpan.FromSeconds(1);

    /// <summary>TestScheduler, der verrät, ob noch Arbeit geplant ist, also ob ein Timer läuft.</summary>
    private sealed class InspectableScheduler : TestScheduler
    {
        public bool HasPendingWork => GetNext() is not null;
    }

    [Fact]
    public void Ohne_Subscribe_startet_kein_Timer()
    {
        var scheduler = new InspectableScheduler();
        Ticker.Create(Second, scheduler);
        Assert.False(scheduler.HasPendingWork);
    }

    [Fact]
    public void Liefert_fuenf_Werte_im_Takt_und_meldet_dann_OnCompleted()
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
    public void Nach_OnCompleted_laeuft_kein_Timer_mehr()
    {
        var scheduler = new InspectableScheduler();
        using var subscription = Ticker.Create(Second, scheduler).Subscribe(_ => { });

        scheduler.AdvanceBy(10 * Second.Ticks);
        Assert.False(scheduler.HasPendingWork);
    }

    [Fact]
    public void Dispose_stoppt_den_Timer()
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
    public void Jeder_Subscriber_bekommt_einen_eigenen_Timer()
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
        Assert.Equal([0, 1], second); // der zweite Timer startet bei 0
    }
}
