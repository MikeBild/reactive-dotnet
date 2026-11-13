namespace Exercises.RxNetTasksAsync;

public class ProductLoaderTests
{
    private static readonly Product Brake = new("p1", "Bremsscheibe", 4_990, 12);

    [Fact]
    public void Without_Subscribe_no_request_starts()
    {
        var calls = 0;
        ProductLoader.Load(_ => { calls++; return Task.FromResult(Brake); });
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task Await_returns_the_result()
    {
        var product = await ProductLoader.Load(_ => Task.FromResult(Brake)).FirstAsync();
        Assert.Equal(Brake, product);
    }

    [Fact]
    public void Dispose_cancels_the_request()
    {
        var cancelled = false;
        var pending = new TaskCompletionSource<Product>();
        var subscription = ProductLoader
            .Load(ct =>
            {
                ct.Register(() => cancelled = true);
                return pending.Task;
            })
            .Subscribe(_ => { }, _ => { });

        Assert.False(cancelled);
        subscription.Dispose();
        Assert.True(cancelled);
    }

    [Fact]
    public void Timeout_signals_an_error_and_cancels_the_request()
    {
        var scheduler = new TestScheduler();
        var cancelled = false;
        Exception? error = null;
        using var subscription = ProductLoader
            .LoadWithTimeout(
                ct =>
                {
                    ct.Register(() => cancelled = true);
                    return new TaskCompletionSource<Product>().Task; // never responds
                },
                TimeSpan.FromSeconds(2),
                scheduler)
            .Subscribe(_ => { }, e => error = e);

        scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
        Assert.Null(error);

        scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
        Assert.IsType<TimeoutException>(error);
        Assert.True(cancelled);
    }

    [Fact]
    public async Task FirstThreeAsync_returns_the_first_three_values_and_unsubscribes()
    {
        var disposed = false;
        var prices = Observable.Range(100, 10).Finally(() => disposed = true);

        var firstThree = await ProductLoader.FirstThreeAsync(prices);

        Assert.Equal([100, 101, 102], firstThree);
        Assert.True(disposed);
    }
}
