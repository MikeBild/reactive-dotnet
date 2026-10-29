namespace Exercises.RxNetColdHot;

public static class CustomerStream
{
    /// <summary>Cold: Jedes Subscribe startet einen eigenen Request.</summary>
    public static IObservable<Customer> Cold(
        Func<CancellationToken, Task<Customer>> load, Action onRequest, IScheduler scheduler) =>
        // Defer ruft die Factory bei jedem Subscribe neu auf, also ein Request pro Subscriber.
        Observable
            .Defer(() => Observable.FromAsync(load, scheduler))
            .Do(_ => onRequest());

    /// <summary>Hot: Alle gleichzeitigen Subscriber teilen sich einen Request.</summary>
    public static IObservable<Customer> Shared(
        Func<CancellationToken, Task<Customer>> load, Action onRequest, IScheduler scheduler) =>
        // Publish verteilt eine Ausführung an alle Subscriber, RefCount startet sie beim ersten
        // Subscriber und beendet sie nach dem letzten. Das entspricht share() in RxJS.
        // Ein Unterschied zu share(): Nach OnCompleted startet RefCount nicht neu, späte
        // Subscriber bekommen nur noch OnCompleted. Brauchen sie den Wert, nimm Replay(1).RefCount().
        Cold(load, onRequest, scheduler)
            .Publish()
            .RefCount();
}
