using System.Reactive.Linq;
using System.Threading.Channels;
using Microsoft.Reactive.Testing;
using ReactiveSamples.Backpressure;

namespace ReactiveSamples.Tests;

public class BackpressureTests : ReactiveTest
{
    /// <summary>Fast producer: one value every 10 ticks, 1..100.</summary>
    private static ITestableObservable<int> FastProducer(TestScheduler scheduler) =>
        scheduler.CreateHotObservable(
            Enumerable.Range(1, 100).Select(i => OnNext(10 * i + 5, i)).ToArray());

    [Fact]
    public void Sample_keeps_only_the_latest_value_per_period()
    {
        var scheduler = new TestScheduler();
        var producer = FastProducer(scheduler);

        var observer = scheduler.Start(
            () => producer.LatestPer(TimeSpan.FromTicks(100), scheduler),
            created: 0, subscribed: 0, disposed: 2000);

        var values = observer.Messages.Select(m => m.Value.Value).ToList();
        Assert.Equal([9, 19, 29, 39, 49, 59, 69, 79, 89, 99, 100], values); // 11 instead of 100 values
    }

    [Fact]
    public void Buffer_bundles_values_into_batches()
    {
        var scheduler = new TestScheduler();
        var producer = FastProducer(scheduler);

        var observer = scheduler.Start(
            () => producer.Batches(TimeSpan.FromTicks(1000), maxCount: 25, scheduler),
            created: 0, subscribed: 0, disposed: 2000);

        var sizes = observer.Messages.Where(m => m.Value.HasValue).Select(m => m.Value.Value.Count).ToList();
        Assert.Equal(100, sizes.Sum()); // nothing gets lost
        Assert.All(sizes, size => Assert.InRange(size, 1, 25));
    }

    [Fact]
    public async Task Bounded_channel_with_DropOldest_keeps_the_newest_values()
    {
        var pulled = new List<int>();
        await foreach (var value in Observable.Range(1, 100).ToBoundedAsyncEnumerable(capacity: 10))
        {
            pulled.Add(value);
        }
        Assert.Equal(Enumerable.Range(91, 10), pulled);
    }

    [Fact]
    public async Task Bounded_channel_with_Wait_slows_the_producer_and_loses_nothing()
    {
        var pulled = new List<int>();
        await foreach (var value in Observable.Range(1, 100).ToBoundedAsyncEnumerable(capacity: 1, BoundedChannelFullMode.Wait))
        {
            pulled.Add(value);
        }
        Assert.Equal(Enumerable.Range(1, 100), pulled);
    }

    [Fact]
    public async Task Consumer_can_stop_early_without_leaving_the_producer_blocked()
    {
        var produced = 0;
        var source = Observable.Range(1, 1_000).Do(_ => Interlocked.Increment(ref produced));

        await foreach (var value in source.ToBoundedAsyncEnumerable(capacity: 1, BoundedChannelFullMode.Wait))
        {
            if (value == 3) break;
        }

        // Without the release, the producer thread would stay stuck in WriteAsync at value 4 or 5 forever.
        // A synchronous producer like Range cannot be canceled in the middle of the loop –
        // it runs to the end, the remaining values are dropped.
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (Volatile.Read(ref produced) < 1_000 && DateTime.UtcNow < deadline)
        {
            await Task.Delay(10);
        }
        Assert.Equal(1_000, Volatile.Read(ref produced));
    }
}
