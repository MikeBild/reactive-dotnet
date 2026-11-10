namespace Exercises.RxNetErrorHandling;

public static class Loader
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(2);

    /// <summary>Pause before the retry with the number attempt (1, 2, …).</summary>
    public static TimeSpan Backoff(int attempt) => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 250);

    public static IObservable<LoadResult<T>> LoadAll<T>(
        IEnumerable<string> urls, Func<string, IObservable<T>> fetch, IScheduler scheduler) =>
        urls.ToObservable().SelectMany(url =>
            fetch(url)
                // Timeout before RetryWhen: Each attempt gets its own two seconds,
                // and a hanging request starts again.
                .Timeout(Timeout, scheduler)
                // RetryWhen gets the errors as a stream. Each value from it triggers a new
                // attempt, an error from it ends the retrying.
                .RetryWhen(errors => errors.SelectMany((error, index) => index < 2
                    ? Observable.Timer(Backoff(index + 1), scheduler)
                    : Observable.Throw<long>(error)))
                .Select(value => (LoadResult<T>)new Loaded<T>(url, value))
                // Catch inside: The error becomes a value, the other requests keep running.
                // Outside on SelectMany, it would end all three requests.
                .Catch((Exception error) => Observable.Return<LoadResult<T>>(new Failed<T>(url, error.Message))));
}
