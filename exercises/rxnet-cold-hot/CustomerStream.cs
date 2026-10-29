// Lektion rxnet-cold-hot: Cold oder Hot, wer startet den Producer?
//
// Aufgabe: Abonniere ein Observable aus Observable.Defer zweimal, zähle die Requests
// mit Do und teile die Ausführung danach mit Publish und RefCount.
//
// load steht für den Request, der den angemeldeten Kunden lädt.

namespace Exercises.RxNetColdHot;

public static class CustomerStream
{
    /// <summary>Cold: Jedes Subscribe startet einen eigenen Request.</summary>
    public static IObservable<Customer> Cold(
        Func<CancellationToken, Task<Customer>> load, Action onRequest, IScheduler scheduler)
    {
        // TODO: Observable.Defer(() => Observable.FromAsync(load, scheduler)) und mit Do je Antwort onRequest aufrufen
        throw new NotImplementedException("TODO: Cold");
    }

    /// <summary>Hot: Alle gleichzeitigen Subscriber teilen sich einen Request.</summary>
    public static IObservable<Customer> Shared(
        Func<CancellationToken, Task<Customer>> load, Action onRequest, IScheduler scheduler)
    {
        // TODO: Cold wiederverwenden und mit Publish().RefCount() teilen
        throw new NotImplementedException("TODO: Shared");
    }
}
