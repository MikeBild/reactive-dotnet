using System.Reactive.Linq;

namespace ReactiveSamples.ObserverBasics;

/// <summary>
/// Modernization of Rx_Observer_Observables (2010/2011).
/// Earlier: a hand-written producer with List&lt;IObserver&gt; whose Subscribe returned "this"
/// (one Dispose removed ALL Observers) and that never sent OnCompleted.
/// Today: Observable.Create takes care of subscription management, unsubscribing, and the Rx protocol
/// (OnNext* (OnError | OnCompleted)?).
/// </summary>
public static class NameProducer
{
    public static readonly IReadOnlyList<string> Names = ["peter", "paul", "maria"];

    /// <summary>Cold Observable: every subscriber gets its own execution.</summary>
    public static IObservable<string> Cold() =>
        Observable.Create<string>(observer =>
        {
            foreach (var name in Names)
            {
                observer.OnNext(name);
            }
            observer.OnCompleted();
            return () => { }; // nothing to clean up
        });

    /// <summary>
    /// Short form: for a fixed sequence, you do not need Observable.Create at all.
    /// Matches RxJS <c>from(["peter", "paul", "maria"])</c>.
    /// </summary>
    public static IObservable<string> FromList() => Names.ToObservable();
}

/// <summary>
/// A custom Observer is rarely needed today – Observer.Create or Subscribe(onNext, onError, onCompleted)
/// are enough. It stays here to make the three channels of the protocol visible.
/// </summary>
public sealed class RecordingObserver<T>(string name) : IObserver<T>
{
    public string Name { get; } = name;
    public List<string> Log { get; } = [];

    public void OnNext(T value) => Log.Add($"{Name}: {value}");
    public void OnError(Exception error) => Log.Add($"{Name}: error {error.Message}"); // earlier: "finished."
    public void OnCompleted() => Log.Add($"{Name}: completed");
}
