namespace Exercises.RxNetErrorHandling;

public static class Loader
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(2);

    /// <summary>Pause vor der Wiederholung mit der Nummer attempt (1, 2, …).</summary>
    public static TimeSpan Backoff(int attempt) => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 250);

    public static IObservable<LoadResult<T>> LoadAll<T>(
        IEnumerable<string> urls, Func<string, IObservable<T>> fetch, IScheduler scheduler) =>
        urls.ToObservable().SelectMany(url =>
            fetch(url)
                // Timeout vor RetryWhen: Jeder Versuch bekommt seine eigenen zwei Sekunden,
                // und ein hängender Request startet neu.
                .Timeout(Timeout, scheduler)
                // RetryWhen bekommt die Fehler als Stream. Jeder Wert daraus löst einen neuen
                // Versuch aus, ein Fehler daraus beendet das Wiederholen.
                .RetryWhen(errors => errors.SelectMany((error, index) => index < 2
                    ? Observable.Timer(Backoff(index + 1), scheduler)
                    : Observable.Throw<long>(error)))
                .Select(value => (LoadResult<T>)new Loaded<T>(url, value))
                // Catch innen: Der Fehler wird zum Wert, die anderen Requests laufen weiter.
                // Außen am SelectMany würde er alle drei Requests beenden.
                .Catch((Exception error) => Observable.Return<LoadResult<T>>(new Failed<T>(url, error.Message))));
}
