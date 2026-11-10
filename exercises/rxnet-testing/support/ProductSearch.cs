// The product search of the workshop shop, which you test in this lesson.
// It is finished, you only write the tests. Sources, service, and scheduler
// come in as parameters. That is why you do not need mocks.

namespace Exercises.RxNetTesting;

public abstract record SearchState(string Term);

public sealed record SearchOk(string Term, IReadOnlyList<string> Results) : SearchState(Term);

public sealed record SearchFailed(string Term) : SearchState(Term);

public static class ProductSearch
{
    public static readonly TimeSpan Quiet = TimeSpan.FromMilliseconds(300);

    public static IObservable<SearchState> Results(
        IObservable<string> terms, Func<string, IObservable<IReadOnlyList<string>>> search, IScheduler scheduler) =>
        terms
            .Throttle(Quiet, scheduler) // in Rx.NET, Throttle is debounceTime from RxJS
            .DistinctUntilChanged()
            .Select(term => search(term)
                .Select(results => (SearchState)new SearchOk(term, results))
                .Catch((Exception _) => Observable.Return<SearchState>(new SearchFailed(term))))
            .Switch();
}
