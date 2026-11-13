namespace Exercises.RxNetErrorHandling;

public class LoaderTests : ReactiveTest
{
    private static readonly long Ms = TimeSpan.TicksPerMillisecond;

    /// <summary>Simulated server: counts attempts per URL and responds depending on the URL.</summary>
    private sealed class FakeServer(TestScheduler scheduler)
    {
        public Dictionary<string, int> Attempts { get; } = [];

        public IObservable<Product> Fetch(string url) => Observable.Defer(() =>
        {
            Attempts[url] = Attempts.GetValueOrDefault(url) + 1;
            return url switch
            {
                "/ok" => Observable.Return(new Product("p1", "Bremsscheibe", 4_990, 3))
                    .Delay(TimeSpan.FromMilliseconds(100), scheduler),
                "/flaky" when Attempts[url] < 3 => Observable.Throw<Product>(new HttpRequestException("503")),
                "/flaky" => Observable.Return(new Product("p2", "Zündkerze", 790, 40)),
                "/broken" => Observable.Throw<Product>(new HttpRequestException("500")),
                _ => Observable.Never<Product>(), // hangs
            };
        });
    }

    private static (List<Recorded<Notification<LoadResult<Product>>>> Messages, FakeServer Server) Run(params string[] urls)
    {
        var scheduler = new TestScheduler();
        var server = new FakeServer(scheduler);
        var observer = scheduler.Start(
            () => Loader.LoadAll(urls, server.Fetch, scheduler),
            created: 0, subscribed: 0, disposed: 60_000 * Ms);
        return (observer.Messages.ToList(), server);
    }

    [Fact]
    public void Success_becomes_a_Loaded_value()
    {
        var (messages, _) = Run("/ok");
        var loaded = Assert.IsType<Loaded<Product>>(messages[0].Value.Value);
        Assert.Equal("Bremsscheibe", loaded.Value.Name);
        Assert.Equal(NotificationKind.OnCompleted, messages[^1].Value.Kind);
    }

    [Fact]
    public void Retries_twice_with_backoff_and_then_succeeds()
    {
        var (messages, server) = Run("/flaky");
        Assert.Equal(3, server.Attempts["/flaky"]);
        Assert.IsType<Loaded<Product>>(messages[0].Value.Value);
        Assert.Equal((500 + 1000) * Ms, messages[0].Time, tolerance: 10); // 500 ms and 1 s pause
    }

    [Fact]
    public void After_two_retries_the_error_becomes_a_Failed_value()
    {
        var (messages, server) = Run("/broken");
        Assert.Equal(3, server.Attempts["/broken"]);
        var failed = Assert.IsType<Failed<Product>>(messages[0].Value.Value);
        Assert.Equal("500", failed.Message);
    }

    [Fact]
    public void A_hanging_request_gets_a_timeout_per_attempt()
    {
        var (messages, server) = Run("/hang");
        Assert.Equal(3, server.Attempts["/hang"]);
        Assert.IsType<Failed<Product>>(messages[0].Value.Value);
        // three attempts of two seconds each plus 500 ms and 1 s pause
        Assert.Equal((3 * 2000 + 500 + 1000) * Ms, messages[0].Time, tolerance: 10);
    }

    [Fact]
    public void An_error_does_not_end_the_other_requests()
    {
        var (messages, _) = Run("/broken", "/ok", "/flaky");
        var results = messages.Where(m => m.Value.HasValue).Select(m => m.Value.Value).ToList();

        Assert.Equal(3, results.Count);
        Assert.Equal(["/broken", "/flaky", "/ok"], results.Select(r => r.Url).Order());
        Assert.Equal(2, results.OfType<Loaded<Product>>().Count());
        Assert.Equal(NotificationKind.OnCompleted, messages[^1].Value.Kind);
    }
}
