namespace Exercises.RxNetColdHot;

public static class CustomerStream
{
    /// <summary>Cold: Every Subscribe starts its own request.</summary>
    public static IObservable<Customer> Cold(
        Func<CancellationToken, Task<Customer>> load, Action onRequest, IScheduler scheduler) =>
        // Defer calls the factory again on every Subscribe, so one request per subscriber.
        Observable
            .Defer(() => Observable.FromAsync(load, scheduler))
            .Do(_ => onRequest());

    /// <summary>Hot: All concurrent subscribers share one request.</summary>
    public static IObservable<Customer> Shared(
        Func<CancellationToken, Task<Customer>> load, Action onRequest, IScheduler scheduler) =>
        // Publish distributes one execution to all subscribers, RefCount starts it with the first
        // subscriber and ends it after the last one. This matches share() in RxJS.
        // One difference from share(): After OnCompleted, RefCount does not restart, late
        // subscribers only get OnCompleted. If they need the value, use Replay(1).RefCount().
        Cold(load, onRequest, scheduler)
            .Publish()
            .RefCount();
}
