// Lektion rxnet-subscription-cleanup: Jede Subscription braucht ein Ende
//
// Aufgabe: Bau eine Komponente mit drei Subscriptions auf Interval und ein Event und
// beende alle gemeinsam über ein CompositeDisposable und TakeUntil.
//
// Die Ansicht einer Bestellung rendert:
//  - jede Sekunde "tick <n>"                 (Interval)
//  - alle fünf Sekunden "autosave <n>"       (Interval)
//  - bei jedem Resize des Fensters "resize"  (Event)
// Beim Dispose der Ansicht meldet jede Subscription mit Finally "<name> gestoppt".

namespace Exercises.RxNetSubscriptionCleanup;

public sealed class OrderView : IDisposable
{
    // TODO: ein Subject<Unit> als Signal fürs Ende und ein CompositeDisposable für die Subscriptions

    public OrderView(AppWindow window, IScheduler scheduler, Action<string> render)
    {
        // TODO: drei Subscriptions, jeweils mit Select, Finally(() => render("… gestoppt"))
        //       und TakeUntil als letztem Operator, gesammelt im CompositeDisposable
        throw new NotImplementedException("TODO: OrderView");
    }

    public void Dispose()
    {
        // TODO: Signal senden, Subject beenden, CompositeDisposable entsorgen
    }
}
