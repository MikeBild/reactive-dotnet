namespace Exercises.RxNetSubscriptionCleanup;

public class OrderViewTests
{
    private static readonly long Second = TimeSpan.TicksPerSecond;

    [Fact]
    public void Rendert_Ticks_Autosave_und_Resize()
    {
        var scheduler = new TestScheduler();
        var window = new AppWindow();
        var lines = new List<string>();
        using var view = new OrderView(window, scheduler, lines.Add);

        scheduler.AdvanceBy(5 * Second);
        window.Resize();

        // Bei Sekunde fünf fallen tick 4 und autosave 0 zusammen, die Reihenfolge dazwischen ist offen.
        Assert.Equal(["autosave 0", "resize", "tick 0", "tick 1", "tick 2", "tick 3", "tick 4"], lines.Order());
        Assert.Equal("resize", lines[^1]);
    }

    [Fact]
    public void Dispose_beendet_alle_drei_Subscriptions()
    {
        var scheduler = new TestScheduler();
        var window = new AppWindow();
        var lines = new List<string>();
        var view = new OrderView(window, scheduler, lines.Add);

        scheduler.AdvanceBy(2 * Second);
        view.Dispose();
        lines.Clear();

        scheduler.AdvanceBy(60 * Second);
        window.Resize();

        Assert.Empty(lines);
    }

    [Fact]
    public void Dispose_meldet_den_Event_Handler_ab()
    {
        var window = new AppWindow();
        var view = new OrderView(window, new TestScheduler(), _ => { });

        Assert.Equal(1, window.HandlerCount);
        view.Dispose();
        Assert.Equal(0, window.HandlerCount);
    }

    [Fact]
    public void Finally_meldet_jedes_Ende()
    {
        var lines = new List<string>();
        var view = new OrderView(new AppWindow(), new TestScheduler(), lines.Add);

        view.Dispose();

        Assert.Equal(["autosave gestoppt", "resize gestoppt", "tick gestoppt"], lines.Order());
    }
}
