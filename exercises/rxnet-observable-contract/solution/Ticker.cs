namespace Exercises.RxNetObservableContract;

public static class Ticker
{
    /// <summary>Zählt ab 0 im Takt von period hoch und meldet nach count Werten OnCompleted.</summary>
    public static IObservable<int> Create(TimeSpan period, IScheduler scheduler, int count = 5) =>
        Observable.Create<int>(observer =>
        {
            // Der Producer startet erst hier, also bei jedem Subscribe neu.
            var value = 0;
            var timer = new SerialDisposable();
            timer.Disposable = scheduler.SchedulePeriodic(period, () =>
            {
                observer.OnNext(value++);
                if (value == count)
                {
                    observer.OnCompleted();
                    timer.Dispose(); // nach OnCompleted läuft kein Timer mehr
                }
            });

            // Das Teardown: Dispose der Subscription stoppt den Timer.
            return timer;
        });
}
