// Lesson rxnet-tasks-async: Tasks and Observables work together
//
// Task: Wrap an async method with a CancellationToken using FromAsync, check in the
// test that Dispose triggers cancellation, and get the result back with await.
//
// load stands for a request, such as HttpClient.GetFromJsonAsync(url, ct).

namespace Exercises.RxNetTasksAsync;

public static class ProductLoader
{
    /// <summary>Starts load only on Subscribe. Dispose cancels the CancellationToken.</summary>
    public static IObservable<Product> Load(Func<CancellationToken, Task<Product>> load)
    {
        // TODO: Observable.FromAsync
        throw new NotImplementedException("TODO: Load");
    }

    /// <summary>Like Load, but after timeout a TimeoutException arrives and the request is canceled.</summary>
    public static IObservable<Product> LoadWithTimeout(
        Func<CancellationToken, Task<Product>> load, TimeSpan timeout, IScheduler scheduler)
    {
        // TODO: Observable.FromAsync(load, scheduler) with Timeout(timeout, scheduler)
        throw new NotImplementedException("TODO: LoadWithTimeout");
    }

    /// <summary>The first three prices as a list, then the Subscription ends.</summary>
    public static Task<IList<int>> FirstThreeAsync(IObservable<int> pricesCents)
    {
        // TODO: Take(3), ToList, and await or ToTask
        throw new NotImplementedException("TODO: FirstThreeAsync");
    }
}
