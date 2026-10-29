namespace Exercises.RxNetSubscriptionCleanup;

public sealed class OrderView : IDisposable
{
    private readonly Subject<Unit> _destroy = new();
    private readonly CompositeDisposable _subscriptions = new();

    public OrderView(AppWindow window, IScheduler scheduler, Action<string> render)
    {
        // TakeUntil steht jeweils zuletzt, damit es alles davor beendet.
        // Finally läuft bei OnCompleted, OnError und Dispose, also bei jedem Ende.
        _subscriptions.Add(Observable
            .Interval(TimeSpan.FromSeconds(1), scheduler)
            .Select(n => $"tick {n}")
            .Finally(() => render("tick gestoppt"))
            .TakeUntil(_destroy)
            .Subscribe(render));

        _subscriptions.Add(Observable
            .Interval(TimeSpan.FromSeconds(5), scheduler)
            .Select(n => $"autosave {n}")
            .Finally(() => render("autosave gestoppt"))
            .TakeUntil(_destroy)
            .Subscribe(render));

        _subscriptions.Add(Observable
            .FromEventPattern(h => window.Resized += h, h => window.Resized -= h)
            .Select(_ => "resize")
            .Finally(() => render("resize gestoppt"))
            .TakeUntil(_destroy)
            .Subscribe(render));
    }

    public void Dispose()
    {
        // Das Signal beendet alle Streams sauber mit OnCompleted.
        // Das CompositeDisposable ist das Sicherheitsnetz für alles, was kein TakeUntil hat.
        _destroy.OnNext(Unit.Default);
        _destroy.OnCompleted();
        _subscriptions.Dispose();
    }
}
