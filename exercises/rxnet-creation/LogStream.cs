// Lektion rxnet-creation: Fast alles wird zum Observable
//
// Aufgabe: Bau mit Creation-Methoden aus einem Event, einem Timer und einem Task
// einen gemeinsamen Log-Stream, in dem jede Zeile ihre Quelle als Präfix trägt.

namespace Exercises.RxNetCreation;

public static class LogStream
{
    /// <summary>Jeder Klick auf den Button wird ein Wert. Dispose meldet den Handler ab.</summary>
    public static IObservable<string> Clicks(Button button)
    {
        // TODO: Observable.FromEventPattern(h => button.Clicked += h, h => button.Clicked -= h)
        //       und mit Select den Text "click" liefern
        throw new NotImplementedException("TODO: Clicks");
    }

    /// <summary>Zählt ab 0 hoch, ein Wert pro period.</summary>
    public static IObservable<long> Ticks(TimeSpan period, IScheduler scheduler)
    {
        // TODO: Observable.Interval mit dem scheduler
        throw new NotImplementedException("TODO: Ticks");
    }

    /// <summary>Lädt den Kunden erst beim Subscribe, für jeden Subscriber neu.</summary>
    public static IObservable<Customer> LoadCustomer(Func<CancellationToken, Task<Customer>> load, IScheduler scheduler)
    {
        // TODO: Observable.FromAsync mit load und scheduler
        throw new NotImplementedException("TODO: LoadCustomer");
    }

    /// <summary>Macht aus jedem Wert eine Zeile "prefix: wert".</summary>
    public static IObservable<string> WithPrefix<T>(this IObservable<T> source, string prefix)
    {
        // TODO: Select
        throw new NotImplementedException("TODO: WithPrefix");
    }

    /// <summary>Alle drei Quellen als ein Log-Stream mit den Präfixen click, tick und customer.</summary>
    public static IObservable<string> Create(
        Button button, TimeSpan period, Func<CancellationToken, Task<Customer>> load, IScheduler scheduler)
    {
        // TODO: Merge aus Clicks, Ticks und dem Namen des Kunden, jeweils mit WithPrefix
        throw new NotImplementedException("TODO: LogStream.Create");
    }
}
