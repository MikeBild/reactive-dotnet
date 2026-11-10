// Lesson rxnet-observable-contract: Nothing happens without Subscribe
//
// Task: Use Observable.Create to write a ticker that counts up on an IScheduler,
// signals OnCompleted after five values, and stops the timer on Dispose.
//
// The timer runs on the IScheduler, not on System.Threading.Timer. This way
// the tests advance time with a TestScheduler, without waiting.

namespace Exercises.RxNetObservableContract;

public static class Ticker
{
    /// <summary>Counts up from 0 every period and signals OnCompleted after count values.</summary>
    public static IObservable<int> Create(TimeSpan period, IScheduler scheduler, int count = 5)
    {
        // TODO: Observable.Create<int>(observer => { … })
        //  - start the timer with scheduler.SchedulePeriodic(period, () => …)
        //  - on every tick, observer.OnNext(value++)
        //  - after count values, observer.OnCompleted() and stop the timer
        //  - return the timer's IDisposable so Dispose stops it
        throw new NotImplementedException("TODO: Ticker.Create");
    }
}
