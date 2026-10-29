using System.Reactive.Linq;

namespace ReactiveSamples.ObserverBasics;

/// <summary>
/// Modernisierung von Rx_Observer_Observables (2010/2011).
/// Früher: handgeschriebener Producer mit List&lt;IObserver&gt;, dessen Subscribe "this" zurückgab
/// (ein Dispose entfernte ALLE Observer) und der nie OnCompleted sendete.
/// Heute: Observable.Create kümmert sich um Abo-Verwaltung, Abmeldung und das Rx-Protokoll
/// (OnNext* (OnError | OnCompleted)?).
/// </summary>
public static class NameProducer
{
    public static readonly IReadOnlyList<string> Names = ["peter", "paul", "maria"];

    /// <summary>Cold Observable: jeder Subscriber bekommt seine eigene Ausführung.</summary>
    public static IObservable<string> Cold() =>
        Observable.Create<string>(observer =>
        {
            foreach (var name in Names)
            {
                observer.OnNext(name);
            }
            observer.OnCompleted();
            return () => { }; // nichts aufzuräumen
        });

    /// <summary>
    /// Kurzform: für eine feste Sequenz braucht man gar kein Observable.Create.
    /// Entspricht RxJS <c>from(["peter", "paul", "maria"])</c>.
    /// </summary>
    public static IObservable<string> FromList() => Names.ToObservable();
}

/// <summary>
/// Ein eigener Observer ist heute selten nötig – Observer.Create oder Subscribe(onNext, onError, onCompleted)
/// reichen. Hier bleibt er, um die drei Kanäle des Protokolls sichtbar zu machen.
/// </summary>
public sealed class RecordingObserver<T>(string name) : IObserver<T>
{
    public string Name { get; } = name;
    public List<string> Log { get; } = [];

    public void OnNext(T value) => Log.Add($"{Name}: {value}");
    public void OnError(Exception error) => Log.Add($"{Name}: error {error.Message}"); // früher: "finished."
    public void OnCompleted() => Log.Add($"{Name}: completed");
}
