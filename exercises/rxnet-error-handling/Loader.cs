// Lektion rxnet-error-handling: Ein Fehler beendet den Stream
//
// Aufgabe: Lade drei URLs mit SelectMany, gib jedem Request ein Timeout, wiederhole
// zweimal mit Backoff und liefere bei Fehlern einen Fehlerwert statt abzubrechen.
//
// Regeln:
//  - Timeout von zwei Sekunden je Versuch
//  - höchstens zwei Wiederholungen, Pause vor Versuch n: 2^n × 250 ms, also 500 ms und 1 s
//  - Catch im inneren Stream, damit die anderen URLs weiterlaufen

namespace Exercises.RxNetErrorHandling;

public static class Loader
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(2);

    /// <summary>Pause vor der Wiederholung mit der Nummer attempt (1, 2, …).</summary>
    public static TimeSpan Backoff(int attempt) => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 250);

    public static IObservable<LoadResult<T>> LoadAll<T>(
        IEnumerable<string> urls, Func<string, IObservable<T>> fetch, IScheduler scheduler)
    {
        // TODO: urls.ToObservable().SelectMany(url => fetch(url)
        //         .Timeout(Timeout, scheduler)
        //         .RetryWhen(errors => …)          zwei Wiederholungen mit Backoff, dann den Fehler weiterreichen
        //         .Select(value => (LoadResult<T>)new Loaded<T>(url, value))
        //         .Catch((Exception error) => …))  Fehler als Failed<T>-Wert
        throw new NotImplementedException("TODO: LoadAll");
    }
}
