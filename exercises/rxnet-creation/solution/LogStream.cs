namespace Exercises.RxNetCreation;

public static class LogStream
{
    /// <summary>Every click on the button becomes a value. Dispose removes the handler.</summary>
    public static IObservable<string> Clicks(Button button) =>
        // FromEventPattern registers a separate handler per subscriber and removes it on Dispose.
        Observable
            .FromEventPattern(h => button.Clicked += h, h => button.Clicked -= h)
            .Select(_ => "click");

    /// <summary>Counts up from 0, one value per period.</summary>
    public static IObservable<long> Ticks(TimeSpan period, IScheduler scheduler) =>
        Observable.Interval(period, scheduler);

    /// <summary>Loads the customer only on Subscribe, anew for every subscriber.</summary>
    public static IObservable<Customer> LoadCustomer(Func<CancellationToken, Task<Customer>> load, IScheduler scheduler) =>
        // FromAsync calls load only on Subscribe and cancels the token on Dispose.
        // A task alone would be eager: It runs from the moment it is called, whether anyone listens or not.
        Observable.FromAsync(load, scheduler);

    /// <summary>Turns every value into a line "prefix: value".</summary>
    public static IObservable<string> WithPrefix<T>(this IObservable<T> source, string prefix) =>
        source.Select(value => $"{prefix}: {value}");

    /// <summary>All three sources as one log stream with the prefixes click, tick, and customer.</summary>
    public static IObservable<string> Create(
        Button button, TimeSpan period, Func<CancellationToken, Task<Customer>> load, IScheduler scheduler) =>
        Observable.Merge(
            Clicks(button).Select(_ => "Klick").WithPrefix("click"),
            Ticks(period, scheduler).WithPrefix("tick"),
            LoadCustomer(load, scheduler).Select(customer => customer.Name).WithPrefix("customer"));
}
