namespace Exercises.RxNetFlattening;

public static class Flattening
{
    /// <summary>Search: only the result for the latest term counts.</summary>
    public static IObservable<IReadOnlyList<Product>> SearchResults(
        IObservable<string> terms, Func<string, IObservable<IReadOnlyList<Product>>> search) =>
        // Switch: A new term makes the running search worthless, so cancel and switch.
        // You can safely cancel read requests.
        terms.Select(search).Switch();

    /// <summary>Autosave: save every change, in the order in which it was made.</summary>
    public static IObservable<T> Autosave<T>(IObservable<T> changes, Func<T, IObservable<T>> save) =>
        // Concat: No change may get lost, and the order must be right.
        // Switch could cancel a half-finished save, Merge could mix up the order.
        changes.Select(save).Concat();

    /// <summary>Upload queue: upload files in parallel, but at most maxConcurrent at a time.</summary>
    public static IObservable<string> UploadQueue(
        IObservable<string> files, Func<string, IObservable<string>> upload, int maxConcurrent) =>
        // Merge with a limit: Uploads are independent, parallel is faster.
        // Without a limit (SelectMany), a hundred files would run as a hundred concurrent requests.
        files.Select(upload).Merge(maxConcurrent);

    /// <summary>Login: A double click must not start a second request while the first one is running.</summary>
    public static IObservable<TSession> LoginOnClick<TSession>(IObservable<Unit> clicks, Func<IObservable<TSession>> login) =>
        // Exhaust: While a login is running, further clicks are ignored.
        clicks.ExhaustMap(_ => login());

    /// <summary>Ignores new values while the inner stream of the previous value is still running.</summary>
    public static IObservable<TResult> ExhaustMap<TSource, TResult>(
        this IObservable<TSource> source, Func<TSource, IObservable<TResult>> selector) =>
        // Defer: Every Subscription gets its own flag, otherwise two subscribers would share the state.
        Observable.Defer(() =>
        {
            var running = 0;
            return source
                .Where(_ => Interlocked.CompareExchange(ref running, 1, 0) == 0)
                .SelectMany(value => selector(value).Finally(() => Volatile.Write(ref running, 0)));
        });
}
