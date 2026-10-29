// Lektion rxnet-observable-contract: Ohne Subscribe passiert nichts
//
// Aufgabe: Schreib mit Observable.Create einen Ticker, der auf einem IScheduler
// hochzählt, nach fünf Werten OnCompleted meldet und beim Dispose den Timer stoppt.
//
// Der Timer läuft über den IScheduler, nicht über System.Threading.Timer. So
// stellen die Tests die Zeit mit einem TestScheduler vor, ohne zu warten.

namespace Exercises.RxNetObservableContract;

public static class Ticker
{
    /// <summary>Zählt ab 0 im Takt von period hoch und meldet nach count Werten OnCompleted.</summary>
    public static IObservable<int> Create(TimeSpan period, IScheduler scheduler, int count = 5)
    {
        // TODO: Observable.Create<int>(observer => { … })
        //  - Timer mit scheduler.SchedulePeriodic(period, () => …) starten
        //  - bei jedem Tick observer.OnNext(wert++)
        //  - nach count Werten observer.OnCompleted() und den Timer stoppen
        //  - das IDisposable des Timers zurückgeben, damit Dispose ihn stoppt
        throw new NotImplementedException("TODO: Ticker.Create");
    }
}
