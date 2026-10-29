// Lektion rxnet-tasks-async: Tasks und Observables arbeiten zusammen
//
// Aufgabe: Bind eine async-Methode mit CancellationToken per FromAsync ein, prüf im
// Test, dass Dispose den Abbruch auslöst, und hol das Ergebnis mit await zurück.
//
// load steht für einen Request, etwa HttpClient.GetFromJsonAsync(url, ct).

namespace Exercises.RxNetTasksAsync;

public static class ProductLoader
{
    /// <summary>Startet load erst beim Subscribe. Dispose bricht das CancellationToken ab.</summary>
    public static IObservable<Product> Load(Func<CancellationToken, Task<Product>> load)
    {
        // TODO: Observable.FromAsync
        throw new NotImplementedException("TODO: Load");
    }

    /// <summary>Wie Load, aber nach timeout kommt eine TimeoutException und der Request bricht ab.</summary>
    public static IObservable<Product> LoadWithTimeout(
        Func<CancellationToken, Task<Product>> load, TimeSpan timeout, IScheduler scheduler)
    {
        // TODO: Observable.FromAsync(load, scheduler) mit Timeout(timeout, scheduler)
        throw new NotImplementedException("TODO: LoadWithTimeout");
    }

    /// <summary>Die ersten drei Preise als Liste, danach endet die Subscription.</summary>
    public static Task<IList<int>> FirstThreeAsync(IObservable<int> pricesCents)
    {
        // TODO: Take(3), ToList und await bzw. ToTask
        throw new NotImplementedException("TODO: FirstThreeAsync");
    }
}
