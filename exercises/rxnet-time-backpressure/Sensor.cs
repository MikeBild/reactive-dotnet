// Lesson rxnet-time-backpressure: What if more events arrive than you can process?
//
// Task: From a fast sensor stream, show a throttled live value with
// Sample and an average per time window with Buffer.
//
// An Observable has no back channel to the producer. You decide what happens to too
// many values: drop them (Sample) or collect them (Buffer).

namespace Exercises.RxNetTimeBackpressure;

public static class Sensor
{
    /// <summary>A sensor that sends perSecond values per second. The nth value is read(n).</summary>
    public static IObservable<double> Create(int perSecond, IScheduler scheduler, Func<long, double> read) =>
        Observable
            .Interval(TimeSpan.FromTicks(TimeSpan.TicksPerSecond / perSecond), scheduler)
            .Select(read);

    /// <summary>Live value: the latest value at the end of each window, everything in between is dropped.</summary>
    public static IObservable<double> LiveValue(this IObservable<double> source, TimeSpan period, IScheduler scheduler)
    {
        // TODO: Sample with period and scheduler
        throw new NotImplementedException("TODO: LiveValue");
    }

    /// <summary>Average per window. No value gets lost, empty windows emit nothing.</summary>
    public static IObservable<double> AveragePer(this IObservable<double> source, TimeSpan period, IScheduler scheduler)
    {
        // TODO: Buffer with period and scheduler, drop empty windows with Where, Average
        throw new NotImplementedException("TODO: AveragePer");
    }
}
