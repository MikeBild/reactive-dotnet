namespace Exercises.RxNetCreation;

public static class LogStream
{
    /// <summary>Jeder Klick auf den Button wird ein Wert. Dispose meldet den Handler ab.</summary>
    public static IObservable<string> Clicks(Button button) =>
        // FromEventPattern registriert pro Subscriber einen eigenen Handler und entfernt ihn beim Dispose.
        Observable
            .FromEventPattern(h => button.Clicked += h, h => button.Clicked -= h)
            .Select(_ => "click");

    /// <summary>Zählt ab 0 hoch, ein Wert pro period.</summary>
    public static IObservable<long> Ticks(TimeSpan period, IScheduler scheduler) =>
        Observable.Interval(period, scheduler);

    /// <summary>Lädt den Kunden erst beim Subscribe, für jeden Subscriber neu.</summary>
    public static IObservable<Customer> LoadCustomer(Func<CancellationToken, Task<Customer>> load, IScheduler scheduler) =>
        // FromAsync ruft load erst beim Subscribe auf und bricht das Token beim Dispose ab.
        // Ein Task allein wäre eager: Er läuft ab dem Aufruf, egal ob jemand zuhört.
        Observable.FromAsync(load, scheduler);

    /// <summary>Macht aus jedem Wert eine Zeile "prefix: wert".</summary>
    public static IObservable<string> WithPrefix<T>(this IObservable<T> source, string prefix) =>
        source.Select(value => $"{prefix}: {value}");

    /// <summary>Alle drei Quellen als ein Log-Stream mit den Präfixen click, tick und customer.</summary>
    public static IObservable<string> Create(
        Button button, TimeSpan period, Func<CancellationToken, Task<Customer>> load, IScheduler scheduler) =>
        Observable.Merge(
            Clicks(button).Select(_ => "Klick").WithPrefix("click"),
            Ticks(period, scheduler).WithPrefix("tick"),
            LoadCustomer(load, scheduler).Select(customer => customer.Name).WithPrefix("customer"));
}
