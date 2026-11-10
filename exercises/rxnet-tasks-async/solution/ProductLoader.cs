namespace Exercises.RxNetTasksAsync;

public static class ProductLoader
{
    /// <summary>Starts load only on Subscribe. Dispose cancels the CancellationToken.</summary>
    public static IObservable<Product> Load(Func<CancellationToken, Task<Product>> load) =>
        // The overload with CancellationToken makes the task lazy and cancelable:
        // Subscribe starts it, Dispose triggers the token. A task that is already running would have neither.
        Observable.FromAsync(load);

    /// <summary>Like Load, but after timeout a TimeoutException arrives and the request is canceled.</summary>
    public static IObservable<Product> LoadWithTimeout(
        Func<CancellationToken, Task<Product>> load, TimeSpan timeout, IScheduler scheduler) =>
        // Timeout disposes the inner Subscription, and so FromAsync cancels the token.
        Observable
            .FromAsync(load, scheduler)
            .Timeout(timeout, scheduler);

    /// <summary>The first three prices as a list, then the Subscription ends.</summary>
    public static async Task<IList<int>> FirstThreeAsync(IObservable<int> pricesCents) =>
        // await on an IObservable returns the last value. ToList turns it into exactly one value.
        // For the first value, there is FirstAsync, the counterpart to firstValueFrom in RxJS.
        await pricesCents.Take(3).ToList();
}
