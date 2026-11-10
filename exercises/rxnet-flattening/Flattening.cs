// Lesson rxnet-flattening: Switch, Concat, Merge, or Exhaust?
//
// Task: Build product search, autosave, upload queue, and login button, each with one
// Flattening strategy, and explain every choice in a code comment.
//
// All services come in as parameters. In the test, a Cold Observable from the
// TestScheduler replaces them, without any mock library.
//
// Rx.NET comes with three strategies: Select(…).Switch(), Select(…).Concat(), and
// Select(…).Merge(maxConcurrent) or SelectMany. There is no exhaustMap like in RxJS,
// so you write it yourself here as an Extension Method.

namespace Exercises.RxNetFlattening;

public static class Flattening
{
    /// <summary>Search: only the result for the latest term counts.</summary>
    public static IObservable<IReadOnlyList<Product>> SearchResults(
        IObservable<string> terms, Func<string, IObservable<IReadOnlyList<Product>>> search)
    {
        // TODO: Which strategy? Reasoning:
        throw new NotImplementedException("TODO: SearchResults");
    }

    /// <summary>Autosave: save every change, in the order in which it was made.</summary>
    public static IObservable<T> Autosave<T>(IObservable<T> changes, Func<T, IObservable<T>> save)
    {
        // TODO: Which strategy? Reasoning:
        throw new NotImplementedException("TODO: Autosave");
    }

    /// <summary>Upload queue: upload files in parallel, but at most maxConcurrent at a time.</summary>
    public static IObservable<string> UploadQueue(
        IObservable<string> files, Func<string, IObservable<string>> upload, int maxConcurrent)
    {
        // TODO: Which strategy? Reasoning:
        throw new NotImplementedException("TODO: UploadQueue");
    }

    /// <summary>Login: A double click must not start a second request while the first one is running.</summary>
    public static IObservable<TSession> LoginOnClick<TSession>(IObservable<Unit> clicks, Func<IObservable<TSession>> login)
    {
        // TODO: Which strategy? Reasoning:
        throw new NotImplementedException("TODO: LoginOnClick");
    }

    /// <summary>Ignores new values while the inner stream of the previous value is still running.</summary>
    public static IObservable<TResult> ExhaustMap<TSource, TResult>(
        this IObservable<TSource> source, Func<TSource, IObservable<TResult>> selector)
    {
        // TODO: One “running” flag per Subscription (Observable.Defer),
        //       Where only lets values through when nothing is running,
        //       SelectMany starts the inner stream, Finally resets the flag.
        throw new NotImplementedException("TODO: ExhaustMap");
    }
}
