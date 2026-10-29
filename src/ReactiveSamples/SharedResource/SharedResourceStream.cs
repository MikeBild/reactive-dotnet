using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ReactiveSamples.SharedResource;

/// <summary>
/// Modernisierung von "LastOneMakesLightOff" (RxSamples, 2013) bzw. Gist "thelastoneturnslightoff":
/// Eine teure Ressource (Verbindung, Datei, Sensor) wird beim ersten Subscriber angelegt
/// und beim letzten Unsubscribe wieder freigegeben – "der Letzte macht das Licht aus".
///
/// Bausteine: Observable.Using (Lebensdauer an das Abo binden) + Publish().RefCount()
/// (RxJS: share()). Statt Debug.WriteLine protokolliert die Ressource in ein Log, damit es testbar ist.
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
