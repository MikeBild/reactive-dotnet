// Lesson rxnet-error-handling: An error ends the stream
//
// Task: Load three URLs with SelectMany, give each request a timeout, retry
// twice with backoff, and return an error value on failure instead of aborting.
//
// Rules:
//  - timeout of two seconds per attempt
//  - at most two retries, pause before attempt n: 2^n × 250 ms, so 500 ms and 1 s
//  - Catch in the inner stream so the other URLs keep running

namespace Exercises.RxNetErrorHandling;

public static class Loader
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(2);

    /// <summary>Pause before the retry with the number attempt (1, 2, …).</summary>
    public static TimeSpan Backoff(int attempt) => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 250);

    public static IObservable<LoadResult<T>> LoadAll<T>(
        IEnumerable<string> urls, Func<string, IObservable<T>> fetch, IScheduler scheduler)
    {
        // TODO: urls.ToObservable().SelectMany(url => fetch(url)
        //         .Timeout(Timeout, scheduler)
        //         .RetryWhen(errors => …)          two retries with backoff, then pass the error on
        //         .Select(value => (LoadResult<T>)new Loaded<T>(url, value))
        //         .Catch((Exception error) => …))  error as a Failed<T> value
        throw new NotImplementedException("TODO: LoadAll");
    }
}
