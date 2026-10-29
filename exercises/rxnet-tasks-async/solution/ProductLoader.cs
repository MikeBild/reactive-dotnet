namespace Exercises.RxNetTasksAsync;

public static class ProductLoader
{
    /// <summary>Startet load erst beim Subscribe. Dispose bricht das CancellationToken ab.</summary>
    public static IObservable<Product> Load(Func<CancellationToken, Task<Product>> load) =>
        // Die Überladung mit CancellationToken macht den Task lazy und abbrechbar:
        // Subscribe startet ihn, Dispose löst das Token aus. Ein fertiger Task hätte beides nicht.
        Observable.FromAsync(load);

    /// <summary>Wie Load, aber nach timeout kommt eine TimeoutException und der Request bricht ab.</summary>
    public static IObservable<Product> LoadWithTimeout(
        Func<CancellationToken, Task<Product>> load, TimeSpan timeout, IScheduler scheduler) =>
        // Timeout meldet die innere Subscription ab, und damit bricht FromAsync das Token ab.
        Observable
            .FromAsync(load, scheduler)
            .Timeout(timeout, scheduler);

    /// <summary>Die ersten drei Preise als Liste, danach endet die Subscription.</summary>
    public static async Task<IList<int>> FirstThreeAsync(IObservable<int> pricesCents) =>
        // await auf ein IObservable liefert den letzten Wert. ToList macht daraus genau einen Wert.
        // Für den ersten Wert gibt es FirstAsync, das Gegenstück zu firstValueFrom in RxJS.
        await pricesCents.Take(3).ToList();
}
