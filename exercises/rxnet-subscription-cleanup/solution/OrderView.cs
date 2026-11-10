namespace Exercises.RxNetSubscriptionCleanup;

public sealed class OrderView : IDisposable
{
    private readonly Subject<Unit> _destroy = new();
    private readonly CompositeDisposable _subscriptions = new();

    public OrderView(AppWindow window, IScheduler scheduler, Action<string> render)
    {
        // TakeUntil always comes last so it ends everything before it.
        // Finally runs on OnCompleted, OnError, and Dispose, so on every kind of end.
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
        // The signal ends all streams cleanly with OnCompleted.
        // The CompositeDisposable is the safety net for everything without TakeUntil.
        _destroy.OnNext(Unit.Default);
        _destroy.OnCompleted();
        _subscriptions.Dispose();
    }
}
