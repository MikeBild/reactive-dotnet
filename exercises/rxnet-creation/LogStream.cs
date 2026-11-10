// Lesson rxnet-creation: Almost everything becomes an Observable
//
// Task: Use creation methods to build, from an event, a timer, and a task,
// one shared log stream in which every line carries its source as a prefix.

namespace Exercises.RxNetCreation;

public static class LogStream
{
    /// <summary>Every click on the button becomes a value. Dispose removes the handler.</summary>
    public static IObservable<string> Clicks(Button button)
    {
        // TODO: Observable.FromEventPattern(h => button.Clicked += h, h => button.Clicked -= h)
        //       and return the text "click" with Select
        throw new NotImplementedException("TODO: Clicks");
    }

    /// <summary>Counts up from 0, one value per period.</summary>
    public static IObservable<long> Ticks(TimeSpan period, IScheduler scheduler)
    {
        // TODO: Observable.Interval with the scheduler
        throw new NotImplementedException("TODO: Ticks");
    }

    /// <summary>Loads the customer only on Subscribe, anew for every subscriber.</summary>
    public static IObservable<Customer> LoadCustomer(Func<CancellationToken, Task<Customer>> load, IScheduler scheduler)
    {
        // TODO: Observable.FromAsync with load and scheduler
        throw new NotImplementedException("TODO: LoadCustomer");
    }

    /// <summary>Turns every value into a line "prefix: value".</summary>
    public static IObservable<string> WithPrefix<T>(this IObservable<T> source, string prefix)
    {
        // TODO: Select
        throw new NotImplementedException("TODO: WithPrefix");
    }

    /// <summary>All three sources as one log stream with the prefixes click, tick, and customer.</summary>
    public static IObservable<string> Create(
        Button button, TimeSpan period, Func<CancellationToken, Task<Customer>> load, IScheduler scheduler)
    {
        // TODO: Merge Clicks, Ticks, and the customer's name, each with WithPrefix
        throw new NotImplementedException("TODO: LogStream.Create");
    }
}
