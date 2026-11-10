// Lesson rxnet-testing: Test time without waiting
//
// Task: Write xUnit tests for the finished product search with TestScheduler and
// virtual time: Throttle, duplicates, cancellation, and the error case.
//
// The search is in support/ProductSearch.cs. Here, the test itself is the task:
// The first test shows the pattern, the others are skipped. For each one, remove
// Skip, write the test, and run it:
//
//   dotnet test exercises/Exercises.Start.Tests --filter "FullyQualifiedName~RxNetTesting"
//
// Cheat sheet for the TestScheduler:
//  - One tick is 100 nanoseconds, one millisecond is 10,000 ticks (Ms below).
//  - CreateHotObservable(OnNext(time, value), …) runs independently of Subscribe (absolute time).
//  - CreateColdObservable(OnNext(time, value), …) starts anew for each Subscribe (relative time).
//  - Start(create, created, subscribed, disposed) returns an Observer with Messages.
//    Without arguments, Start subscribes at tick 200 and disposes at tick 1000, so after 0.1 ms.
//    So always pass explicit times.

namespace Exercises.RxNetTesting;

public class ProductSearchTests : ReactiveTest
{
    private const long Ms = TimeSpan.TicksPerMillisecond;

    [Fact]
    public void Interval_liefert_fuenf_Werte_im_Sekundentakt_ohne_echte_Wartezeit()
    {
        var scheduler = new TestScheduler();

        var observer = scheduler.Start(
            () => Observable.Interval(TimeSpan.FromSeconds(1), scheduler).Take(5),
            created: 0, subscribed: 0, disposed: 10_000 * Ms);

        Assert.Equal(
            [
                OnNext(1_000 * Ms + 1, 0L), OnNext(2_000 * Ms + 1, 1L), OnNext(3_000 * Ms + 1, 2L),
                OnNext(4_000 * Ms + 1, 3L), OnNext(5_000 * Ms + 1, 4L), OnCompleted<long>(5_000 * Ms + 1),
            ],
            observer.Messages);
    }

    [Fact(Skip = "TODO: test Throttle")]
    public void Die_Suche_wartet_300_ms_Ruhe_ab_und_sucht_nur_den_letzten_Begriff()
    {
        // TODO: Hot Observable with "B" at 10 ms and "Brems" at 20 ms,
        //       search as a Cold Observable that returns ["Bremsscheibe"] after 1 ms.
        //       Expected: exactly one search for "Brems", result at 321 ms.
    }

    [Fact(Skip = "TODO: test duplicates")]
    public void Derselbe_Begriff_zweimal_hintereinander_loest_keine_zweite_Suche_aus()
    {
        // TODO: "Kerze" at 10 ms and once more at 500 ms, count the searches.
    }

    [Fact(Skip = "TODO: test cancellation")]
    public void Ein_neuer_Begriff_bricht_die_laufende_Suche_ab()
    {
        // TODO: "Brems" with a slow search (500 ms), "Zünd" 400 ms later with a fast search.
        //       Check with slow.Subscriptions that the slow search was unsubscribed.
    }

    [Fact(Skip = "TODO: test the error case")]
    public void Ein_Fehler_der_Suche_wird_zum_Fehlerzustand_und_die_naechste_Suche_laeuft()
    {
        // TODO: The search for "kaputt" signals OnError, the one for "Kerze" returns a result.
        //       Expected: first SearchFailed, then SearchOk.
    }
}
