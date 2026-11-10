namespace Exercises.RxNetObservableContract;

public static class Ticker
{
    /// <summary>Counts up from 0 every period and signals OnCompleted after count values.</summary>
    public static IObservable<int> Create(TimeSpan period, IScheduler scheduler, int count = 5) =>
        Observable.Create<int>(observer =>
        {
            // The producer only starts here, so anew on every Subscribe.
            var value = 0;
            var timer = new SerialDisposable();
            timer.Disposable = scheduler.SchedulePeriodic(period, () =>
            {
                observer.OnNext(value++);
                if (value == count)
                {
                    observer.OnCompleted();
                    timer.Dispose(); // no timer runs after OnCompleted
                }
            });

            // The teardown: Disposing the Subscription stops the timer.
            return timer;
        });
}
