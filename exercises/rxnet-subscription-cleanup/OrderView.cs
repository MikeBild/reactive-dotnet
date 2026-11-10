// Lesson rxnet-subscription-cleanup: Every subscription needs an end
//
// Task: Build a component with three subscriptions to Interval and an event, and
// end them all together through a CompositeDisposable and TakeUntil.
//
// The order view renders:
//  - every second "tick <n>"                 (Interval)
//  - every five seconds "autosave <n>"       (Interval)
//  - on every window resize "resize"         (Event)
// When the view is disposed, every Subscription reports "<name> gestoppt" (stopped) with Finally.

namespace Exercises.RxNetSubscriptionCleanup;

public sealed class OrderView : IDisposable
{
    // TODO: a Subject<Unit> as the signal for the end and a CompositeDisposable for the subscriptions

    public OrderView(AppWindow window, IScheduler scheduler, Action<string> render)
    {
        // TODO: three subscriptions, each with Select, Finally(() => render("… gestoppt"))
        //       and TakeUntil as the last Operator, collected in the CompositeDisposable
        throw new NotImplementedException("TODO: OrderView");
    }

    public void Dispose()
    {
        // TODO: Send the signal, complete the Subject, dispose the CompositeDisposable
    }
}
