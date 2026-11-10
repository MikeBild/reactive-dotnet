using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Channels;

namespace ReactiveSamples.Backpressure;

/// <summary>
/// New (no original from 2010–2013): Rx.NET is pure push and, like RxJS, has no Backpressure.
/// If the consumer is slower than the producer, there are two ways:
/// 1. Reduce lossily in the stream: Sample, Throttle (= RxJS debounceTime), Buffer.
/// 2. Switch to pull: IAsyncEnumerable over a bounded Channel&lt;T&gt;.
///    The consumer sets the pace; what happens when the buffer is full is explicit (BoundedChannelFullMode).
/// </summary>
public static class BackpressureExtensions
{
    /// <summary>Pass on only the latest value per time window (RxJS: sampleTime).</summary>
    public static IObservable<T> LatestPer<T>(this IObservable<T> source, TimeSpan period, IScheduler scheduler) =>
        source.Sample(period, scheduler);

    /// <summary>Group values into batches – fewer but larger work packages (RxJS: bufferTime/bufferCount).</summary>
    public static IObservable<IList<T>> Batches<T>(this IObservable<T> source, TimeSpan span, int maxCount, IScheduler scheduler) =>
        source.Buffer(span, maxCount, scheduler).Where(batch => batch.Count > 0);

    /// <summary>
    /// Push → pull: Values land in a bounded Channel, the consumer reads with await foreach.
    /// With DropOldest, the producer never gets stuck but deliberately loses the oldest values;
    /// with Wait, OnNext blocks until there is room – this slows down a synchronous producer (real Backpressure),
    /// but is usually not a good idea for a timer or event producer.
    /// </summary>
    public static async IAsyncEnumerable<T> ToBoundedAsyncEnumerable<T>(
        this IObservable<T> source,
        int capacity,
        BoundedChannelFullMode fullMode = BoundedChannelFullMode.DropOldest,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateBounded<T>(new BoundedChannelOptions(capacity)
        {
            FullMode = fullMode,
            SingleReader = true,
            SingleWriter = true,
        });

        // With Wait, the producer must not run on the reader's thread, otherwise a
        // synchronous producer (e.g. Observable.Range) waits for a reader that is not reading yet.
        // SubscribeOn moves subscribing (and with it the synchronous emission) to the thread pool.
        var producer = fullMode == BoundedChannelFullMode.Wait ? source.SubscribeOn(TaskPoolScheduler.Default) : source;

        // If the consumer stops early (break, exception, cancellation), a producer waiting in
        // WriteAsync must not stay stuck forever.
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var subscription = producer.Subscribe(
            value =>
            {
                if (fullMode != BoundedChannelFullMode.Wait)
                {
                    channel.Writer.TryWrite(value);
                    return;
                }
                try
                {
                    channel.Writer.WriteAsync(value, stop.Token).AsTask().GetAwaiter().GetResult();
                }
                catch (OperationCanceledException)
                {
                    // Consumer is gone – drop the value.
                }
            },
            error => channel.Writer.TryComplete(error),
            () => channel.Writer.TryComplete());

        try
        {
            await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return item;
            }
        }
        finally
        {
            subscription.Dispose(); // unsubscribe first …
            stop.Cancel();          // … then release a producer that may be waiting in WriteAsync
        }
    }
}
