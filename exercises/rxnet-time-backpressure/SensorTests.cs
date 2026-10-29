namespace Exercises.RxNetTimeBackpressure;

public class SensorTests : ReactiveTest
{
    private static readonly TimeSpan Second = TimeSpan.FromSeconds(1);

    [Fact]
    public void Der_Sensor_schickt_tausend_Werte_pro_Sekunde()
    {
        var scheduler = new TestScheduler();
        var count = 0;
        using var subscription = Sensor.Create(1_000, scheduler, n => n).Subscribe(_ => count++);

        scheduler.AdvanceBy(Second.Ticks);

        Assert.Equal(1_000, count);
    }

    [Fact]
    public void LiveValue_liefert_pro_Sekunde_nur_den_neuesten_Wert()
    {
        var scheduler = new TestScheduler();
        var values = new List<double>();
        using var subscription = Sensor.Create(1_000, scheduler, n => n)
            .LiveValue(Second, scheduler)
            .Subscribe(values.Add);

        scheduler.AdvanceBy(3 * Second.Ticks);

        Assert.Equal(3, values.Count); // statt dreitausend Werten
        Assert.True(values[0] >= 990, $"Erwartet war ein Wert vom Ende des Fensters, nicht {values[0]}.");
        Assert.True(values[1] > values[0] && values[2] > values[1]);
    }

    [Fact]
    public void AveragePer_verwirft_nichts_und_liefert_den_Durchschnitt_pro_Fenster()
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
    public void AveragePer_laesst_leere_Fenster_weg()
    {
        var scheduler = new TestScheduler();
        var source = scheduler.CreateHotObservable(OnNext(3 * Second.Ticks + 1, 5.0));
        var values = new List<double>();
        using var subscription = source.AveragePer(Second, scheduler).Subscribe(values.Add);

        scheduler.AdvanceBy(4 * Second.Ticks + 1);

        Assert.Equal([5.0], values);
    }
}
