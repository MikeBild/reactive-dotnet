// Lesson rxnet-cold-hot: Cold or hot, who starts the producer?
//
// Task: Subscribe twice to an Observable from Observable.Defer, count the requests
// with Do, and then share the execution with Publish and RefCount.
//
// load stands for the request that loads the signed-in customer.

namespace Exercises.RxNetColdHot;

public static class CustomerStream
{
    /// <summary>Cold: Every Subscribe starts its own request.</summary>
    public static IObservable<Customer> Cold(
        Func<CancellationToken, Task<Customer>> load, Action onRequest, IScheduler scheduler)
    {
        // TODO: Observable.Defer(() => Observable.FromAsync(load, scheduler)) and call onRequest with Do for each response
        throw new NotImplementedException("TODO: Cold");
    }

    /// <summary>Hot: All concurrent subscribers share one request.</summary>
    public static IObservable<Customer> Shared(
        Func<CancellationToken, Task<Customer>> load, Action onRequest, IScheduler scheduler)
    {
        // TODO: Reuse Cold and share it with Publish().RefCount()
        throw new NotImplementedException("TODO: Shared");
    }
}
