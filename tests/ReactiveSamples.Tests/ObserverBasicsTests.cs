using System.Reactive;
using System.Reactive.Linq;
using ReactiveSamples.ObserverBasics;

namespace ReactiveSamples.Tests;

public class ObserverBasicsTests
{
    [Fact]
    public void Cold_observable_runs_once_per_subscriber_and_completes()
    {
        var a = new RecordingObserver<string>("a");
        var b = new RecordingObserver<string>("b");

        NameProducer.Cold().Subscribe(a);
        NameProducer.Cold().Subscribe(b);

        Assert.Equal(["a: peter", "a: paul", "a: maria", "a: completed"], a.Log);
        Assert.Equal(4, b.Log.Count);
    }

    [Fact]
    public void Disposing_one_subscription_keeps_the_others_alive()
    {
        // Im Original gab Subscribe "this" zurück: ein Dispose entfernte alle Observer.
        var a = new RecordingObserver<string>("a");
        var b = new RecordingObserver<string>("b");
        var hot = NameProducer.Cold().Publish(); // RxJS: connectable / multicast

        var subscriptionA = hot.Subscribe(a);
        hot.Subscribe(b);
        subscriptionA.Dispose();
        hot.Connect();

        Assert.Empty(a.Log);
        Assert.Equal(["b: peter", "b: paul", "b: maria", "b: completed"], b.Log);
    }

    [Fact]
    public async Task Create_and_ToObservable_are_equivalent()
    {
        Assert.Equal(await NameProducer.FromList().ToList(), await NameProducer.Cold().ToList());
    }

    [Fact]
    public void Observer_Create_replaces_handwritten_observers()
    {
        var log = new List<string>();
        NameProducer.Cold().Subscribe(Observer.Create<string>(log.Add, ex => log.Add(ex.Message), () => log.Add("done")));
        Assert.Equal(["peter", "paul", "maria", "done"], log);
    }
}
