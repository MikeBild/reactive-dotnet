namespace Exercises.RxNetTesting;

public class ProductSearchTests : ReactiveTest
{
    private const long Ms = TimeSpan.TicksPerMillisecond;

    private static IReadOnlyList<string> Results(params string[] names) => names;

    /// <summary>Starts the search at tick 0 and keeps it open for ten seconds of virtual time.</summary>
    private static ITestableObserver<SearchState> Run(
        TestScheduler scheduler, IObservable<string> terms, Func<string, IObservable<IReadOnlyList<string>>> search) =>
        scheduler.Start(
            () => ProductSearch.Results(terms, search, scheduler),
            created: 0, subscribed: 0, disposed: 10_000 * Ms);

    [Fact]
    public void Interval_emits_five_values_one_second_apart_without_real_waiting()
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
    public void The_search_waits_for_300_ms_of_quiet_and_searches_only_the_last_term()
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
        Assert.Equal(321 * Ms, message.Time); // 20 ms + 300 ms quiet + 1 ms search
        Assert.Equal(["Bremsscheibe"], Assert.IsType<SearchOk>(message.Value.Value).Results);
    }

    [Fact]
    public void The_same_term_twice_in_a_row_does_not_trigger_a_second_search()
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
    public void A_new_term_cancels_the_running_search()
    {
        var scheduler = new TestScheduler();
        var terms = scheduler.CreateHotObservable(OnNext(10 * Ms, "Brems"), OnNext(410 * Ms, "Zünd"));
        var slow = scheduler.CreateColdObservable(OnNext(500 * Ms, Results("Bremsscheibe")));
        var fast = scheduler.CreateColdObservable(OnNext(1 * Ms, Results("Zündkerze")));

        var observer = Run(scheduler, terms, term => term == "Brems" ? slow : fast);

        // Brems: quiet until 310 ms, search starts. Zünd: quiet until 710 ms, Brems is canceled.
        Assert.Equal(Subscribe(310 * Ms, 710 * Ms), Assert.Single(slow.Subscriptions));
        var message = Assert.Single(observer.Messages);
        Assert.Equal(711 * Ms, message.Time);
        Assert.Equal("Zünd", message.Value.Value.Term);
    }

    [Fact]
    public void A_search_error_becomes_an_error_state_and_the_next_search_runs()
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
