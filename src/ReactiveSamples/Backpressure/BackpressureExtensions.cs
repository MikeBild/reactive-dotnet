using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Channels;

namespace ReactiveSamples.Backpressure;

/// <summary>
/// Neu (kein Original von 2010–2013): Rx.NET ist reines Push und kennt – wie RxJS – keine Backpressure.
/// Ist der Consumer langsamer als der Producer, gibt es zwei Wege:
/// 1. Verlustbehaftet im Strom reduzieren: Sample, Throttle (= RxJS debounceTime), Buffer.
/// 2. Auf Pull umstellen: IAsyncEnumerable über einen begrenzten Channel&lt;T&gt;.
///    Der Consumer bestimmt das Tempo; was bei vollem Puffer passiert, ist explizit (BoundedChannelFullMode).
/// </summary>
public static class BackpressureExtensions
{
    /// <summary>Nur den jeweils letzten Wert pro Zeitfenster weitergeben (RxJS: sampleTime).</summary>
    public static IObservable<T> LatestPer<T>(this IObservable<T> source, TimeSpan period, IScheduler scheduler) =>
        source.Sample(period, scheduler);

    /// <summary>Werte in Batches bündeln – weniger, dafür größere Arbeitspakete (RxJS: bufferTime/bufferCount).</summary>
    public static IObservable<IList<T>> Batches<T>(this IObservable<T> source, TimeSpan span, int maxCount, IScheduler scheduler) =>
        source.Buffer(span, maxCount, scheduler).Where(batch => batch.Count > 0);

    /// <summary>
    /// Push → Pull: Werte landen in einem begrenzten Channel, der Consumer liest per await foreach.
    /// Mit DropOldest bleibt der Producer nie hängen, verliert aber bewusst die ältesten Werte;
    /// mit Wait blockiert OnNext, bis Platz frei ist – das bremst einen synchronen Producer (echte Backpressure),
    /// ist aber bei einem Timer-/Event-Producer meist keine gute Idee.
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

        // Bei Wait darf der Producer nicht auf dem Thread des Lesers laufen, sonst wartet ein
        // synchroner Producer (z. B. Observable.Range) auf einen Leser, der noch gar nicht liest.
        // SubscribeOn verlegt das Abonnieren (und damit die synchrone Emission) auf den Thread-Pool.
        var producer = fullMode == BoundedChannelFullMode.Wait ? source.SubscribeOn(TaskPoolScheduler.Default) : source;

        // Bricht der Consumer vorzeitig ab (break, Exception, Cancellation), darf ein in WriteAsync
        // wartender Producer nicht für immer hängen bleiben.
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
                    // Consumer ist weg – Wert verwerfen.
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
            subscription.Dispose(); // erst abmelden …
            stop.Cancel();          // … dann einen evtl. in WriteAsync wartenden Producer freigeben
        }
    }
}
