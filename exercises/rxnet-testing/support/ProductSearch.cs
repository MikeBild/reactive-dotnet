// Die Produktsuche des Werkstatt-Shops, die du in dieser Lektion testest.
// Sie ist fertig, du schreibst nur die Tests. Quellen, Service und Scheduler
// kommen als Parameter rein. Deshalb brauchst du keine Mocks.

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
            .Throttle(Quiet, scheduler) // Throttle ist in Rx.NET das debounceTime aus RxJS
            .DistinctUntilChanged()
            .Select(term => search(term)
                .Select(results => (SearchState)new SearchOk(term, results))
                .Catch((Exception _) => Observable.Return<SearchState>(new SearchFailed(term))))
            .Switch();
}
