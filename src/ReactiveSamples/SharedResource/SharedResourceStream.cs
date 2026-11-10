using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ReactiveSamples.SharedResource;

/// <summary>
/// Modernization of "LastOneMakesLightOff" (RxSamples, 2013), also gist "thelastoneturnslightoff":
/// An expensive resource (connection, file, sensor) is created with the first subscriber
/// and released again on the last unsubscribe – "the last one turns off the light".
///
/// Building blocks: Observable.Using (bind the lifetime to the subscription) + Publish().RefCount()
/// (RxJS: share()). Instead of Debug.WriteLine, the resource writes to a log so it is testable.
/// </summary>
public sealed class SharedResource(IList<string> log) : IDisposable
{
    public int SequenceNumber { get; private set; }

    public static SharedResource Create(IList<string> log)
    {
        log.Add("resource created");
        return new SharedResource(log);
    }

    public int Increment() => ++SequenceNumber;

    public void Dispose()
    {
        SequenceNumber = 0;
        log.Add("resource disposed");
    }
}

public static class SharedResourceStream
{
    public static IObservable<int> Create(IList<string> log, IScheduler scheduler, int ticks = 10) =>
        Observable
            .Using(
                () => SharedResource.Create(log),
                resource => Observable
                    .Interval(TimeSpan.FromSeconds(1), scheduler)
                    .Take(ticks)
                    .Select(_ => resource.Increment()))
            .Publish()
            .RefCount();
}
