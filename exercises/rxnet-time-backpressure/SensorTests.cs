namespace Exercises.RxNetTimeBackpressure;

public class SensorTests : ReactiveTest
{
    private static readonly TimeSpan Second = TimeSpan.FromSeconds(1);

    [Fact]
    public void The_sensor_sends_one_thousand_values_per_second()
    {
        var scheduler = new TestScheduler();
        var count = 0;
        using var subscription = Sensor.Create(1_000, scheduler, n => n).Subscribe(_ => count++);

        scheduler.AdvanceBy(Second.Ticks);

        Assert.Equal(1_000, count);
    }

    [Fact]
    public void LiveValue_emits_only_the_latest_value_per_second()
    {
        var scheduler = new TestScheduler();
        var values = new List<double>();
        using var subscription = Sensor.Create(1_000, scheduler, n => n)
            .LiveValue(Second, scheduler)
            .Subscribe(values.Add);

        scheduler.AdvanceBy(3 * Second.Ticks);

        Assert.Equal(3, values.Count); // instead of three thousand values
        Assert.True(values[0] >= 990, $"Expected a value from the end of the window, not {values[0]}. Sample emits the latest value per window.");
        Assert.True(values[1] > values[0] && values[2] > values[1]);
    }

    [Fact]
    public void AveragePer_drops_nothing_and_emits_the_average_per_window()
    {
        var scheduler = new TestScheduler();
        var source = scheduler.CreateHotObservable(
            OnNext(Second.Ticks / 4, 10.0),
            OnNext(Second.Ticks / 2, 20.0),
            OnNext(Second.Ticks + 1, 40.0));
        var values = new List<double>();
        using var subscription = source.AveragePer(Second, scheduler).Subscribe(values.Add);

        scheduler.AdvanceBy(2 * Second.Ticks);

        Assert.Equal([15.0, 40.0], values);
    }

    [Fact]
    public void AveragePer_skips_empty_windows()
    {
        var scheduler = new TestScheduler();
        var source = scheduler.CreateHotObservable(OnNext(3 * Second.Ticks + 1, 5.0));
        var values = new List<double>();
        using var subscription = source.AveragePer(Second, scheduler).Subscribe(values.Add);

        scheduler.AdvanceBy(4 * Second.Ticks + 1);

        Assert.Equal([5.0], values);
    }
}
