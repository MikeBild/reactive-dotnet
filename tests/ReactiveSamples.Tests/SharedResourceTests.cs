using Microsoft.Reactive.Testing;
using ReactiveSamples.SharedResource;

namespace ReactiveSamples.Tests;

public class SharedResourceTests
{
    private static readonly long Second = TimeSpan.FromSeconds(1).Ticks;

    [Fact]
    public void Resource_is_created_once_and_disposed_by_the_last_subscriber()
    {
        var scheduler = new TestScheduler();
        var log = new List<string>();
        var stream = SharedResourceStream.Create(log, scheduler);
        var first = new List<int>();
        var second = new List<int>();

        var firstSubscription = stream.Subscribe(first.Add);
        var secondSubscription = stream.Subscribe(second.Add);
        Assert.Equal(["resource created"], log); // nur EINE Ressource für zwei Subscriber

        scheduler.AdvanceBy(3 * Second);
        firstSubscription.Dispose();
        Assert.Equal(["resource created"], log); // der Zweite hört noch zu – Licht bleibt an

        scheduler.AdvanceBy(2 * Second);
        secondSubscription.Dispose();
        Assert.Equal(["resource created", "resource disposed"], log); // der Letzte macht das Licht aus

        Assert.Equal([1, 2, 3], first);
        Assert.Equal([1, 2, 3, 4, 5], second);
    }

    [Fact]
    public void Resubscribing_after_refcount_zero_creates_a_fresh_resource()
    {
        var scheduler = new TestScheduler();
        var log = new List<string>();
        var stream = SharedResourceStream.Create(log, scheduler);

        stream.Subscribe(_ => { }).Dispose();
        var values = new List<int>();
        stream.Subscribe(values.Add);
        scheduler.AdvanceBy(2 * Second);

        Assert.Equal(["resource created", "resource disposed", "resource created"], log);
        Assert.Equal([1, 2], values); // Zähler beginnt wieder bei 1
    }

    [Fact]
    public void Completion_disposes_the_resource_exactly_once()
    {
        var scheduler = new TestScheduler();
        var log = new List<string>();
        var stream = SharedResourceStream.Create(log, scheduler, ticks: 3);
        var completed = 0;

        stream.Subscribe(_ => { }, () => completed++);
        stream.Subscribe(_ => { }, () => completed++);
        scheduler.Start();

        Assert.Equal(2, completed);
        Assert.Equal(["resource created", "resource disposed"], log);
    }
}
