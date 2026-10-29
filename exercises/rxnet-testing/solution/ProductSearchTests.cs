namespace Exercises.RxNetTesting;

public class ProductSearchTests : ReactiveTest
{
    private const long Ms = TimeSpan.TicksPerMillisecond;

    private static IReadOnlyList<string> Results(params string[] names) => names;

    /// <summary>Startet die Suche ab Tick 0 und hält sie zehn Sekunden virtuelle Zeit offen.</summary>
    private static ITestableObserver<SearchState> Run(
        TestScheduler scheduler, IObservable<string> terms, Func<string, IObservable<IReadOnlyList<string>>> search) =>
        scheduler.Start(
            () => ProductSearch.Results(terms, search, scheduler),
            created: 0, subscribed: 0, disposed: 10_000 * Ms);

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

    [Fact]
    public void Die_Suche_wartet_300_ms_Ruhe_ab_und_sucht_nur_den_letzten_Begriff()
    {
        var scheduler = new TestScheduler();
        var searched = new List<string>();
        var terms = scheduler.CreateHotObservable(OnNext(10 * Ms, "B"), OnNext(20 * Ms, "Brems"));
        IObservable<IReadOnlyList<string>> Search(string term)
        {
            searched.Add(term);
            return scheduler.CreateColdObservable(OnNext(1 * Ms, Results($"{term}scheibe")));
        }

        var observer = Run(scheduler, terms, Search);

        Assert.Equal(["Brems"], searched);
        var message = Assert.Single(observer.Messages);
        Assert.Equal(321 * Ms, message.Time); // 20 ms + 300 ms Ruhe + 1 ms Suche
        Assert.Equal(["Bremsscheibe"], Assert.IsType<SearchOk>(message.Value.Value).Results);
    }

    [Fact]
    public void Derselbe_Begriff_zweimal_hintereinander_loest_keine_zweite_Suche_aus()
    {
        var scheduler = new TestScheduler();
        var searches = 0;
        var terms = scheduler.CreateHotObservable(OnNext(10 * Ms, "Kerze"), OnNext(500 * Ms, "Kerze"));
        IObservable<IReadOnlyList<string>> Search(string term)
        {
            searches++;
            return scheduler.CreateColdObservable(OnNext(1 * Ms, Results(term)));
        }

        var observer = Run(scheduler, terms, Search);

        Assert.Equal(1, searches);
        Assert.Single(observer.Messages);
    }

    [Fact]
    public void Ein_neuer_Begriff_bricht_die_laufende_Suche_ab()
    {
        var scheduler = new TestScheduler();
        var terms = scheduler.CreateHotObservable(OnNext(10 * Ms, "Brems"), OnNext(410 * Ms, "Zünd"));
        var slow = scheduler.CreateColdObservable(OnNext(500 * Ms, Results("Bremsscheibe")));
        var fast = scheduler.CreateColdObservable(OnNext(1 * Ms, Results("Zündkerze")));

        var observer = Run(scheduler, terms, term => term == "Brems" ? slow : fast);

        // Brems: Ruhe bis 310 ms, Suche startet. Zünd: Ruhe bis 710 ms, Brems wird abgebrochen.
        Assert.Equal(Subscribe(310 * Ms, 710 * Ms), Assert.Single(slow.Subscriptions));
        var message = Assert.Single(observer.Messages);
        Assert.Equal(711 * Ms, message.Time);
        Assert.Equal("Zünd", message.Value.Value.Term);
    }

    [Fact]
    public void Ein_Fehler_der_Suche_wird_zum_Fehlerzustand_und_die_naechste_Suche_laeuft()
    {
        var scheduler = new TestScheduler();
        var terms = scheduler.CreateHotObservable(OnNext(10 * Ms, "kaputt"), OnNext(500 * Ms, "Kerze"));
        IObservable<IReadOnlyList<string>> Search(string term) => term == "kaputt"
            ? scheduler.CreateColdObservable(OnError<IReadOnlyList<string>>(1 * Ms, new HttpRequestException("500")))
            : scheduler.CreateColdObservable(OnNext(1 * Ms, Results(term)));

        var observer = Run(scheduler, terms, Search);

        Assert.Equal(2, observer.Messages.Count);
        Assert.Equal(new SearchFailed("kaputt"), observer.Messages[0].Value.Value);
        Assert.Equal(311 * Ms, observer.Messages[0].Time);
        Assert.Equal("Kerze", Assert.IsType<SearchOk>(observer.Messages[1].Value.Value).Term);
        Assert.Equal(801 * Ms, observer.Messages[1].Time);
    }
}
