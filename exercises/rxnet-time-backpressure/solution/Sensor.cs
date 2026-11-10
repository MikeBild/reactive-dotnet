namespace Exercises.RxNetTimeBackpressure;

public static class Sensor
{
    /// <summary>A sensor that sends perSecond values per second. The nth value is read(n).</summary>
    public static IObservable<double> Create(int perSecond, IScheduler scheduler, Func<long, double> read) =>
        Observable
            .Interval(TimeSpan.FromTicks(TimeSpan.TicksPerSecond / perSecond), scheduler)
            .Select(read);

    /// <summary>Live value: the latest value at the end of each window, everything in between is dropped.</summary>
    public static IObservable<double> LiveValue(this IObservable<double> source, TimeSpan period, IScheduler scheduler) =>
        // Sample matches auditTime in RxJS: the latest value per window.
        // Throttle would be wrong: In Rx.NET, Throttle is debounceTime from RxJS and waits for a quiet period,
        // which never comes with a sensor.
        source.Sample(period, scheduler);

    /// <summary>Average per window. No value gets lost, empty windows emit nothing.</summary>
    public static IObservable<double> AveragePer(this IObservable<double> source, TimeSpan period, IScheduler scheduler) =>
        // Buffer drops nothing, it collects. The price: The display lags one window behind.
        source
            .Buffer(period, scheduler)
            .Where(batch => batch.Count > 0)
            .Select(batch => batch.Average());
}
