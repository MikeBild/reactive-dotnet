namespace Exercises.RxNetTimeBackpressure;

public static class Sensor
{
    /// <summary>Ein Sensor, der perSecond Werte pro Sekunde schickt. Der n-te Wert ist read(n).</summary>
    public static IObservable<double> Create(int perSecond, IScheduler scheduler, Func<long, double> read) =>
        Observable
            .Interval(TimeSpan.FromTicks(TimeSpan.TicksPerSecond / perSecond), scheduler)
            .Select(read);

    /// <summary>Live-Wert: am Ende jedes Fensters der neueste Wert, alles dazwischen fällt weg.</summary>
    public static IObservable<double> LiveValue(this IObservable<double> source, TimeSpan period, IScheduler scheduler) =>
        // Sample entspricht auditTime in RxJS: pro Fenster der neueste Wert.
        // Throttle wäre falsch: Throttle ist in Rx.NET das debounceTime aus RxJS und wartet auf Ruhe,
        // die bei einem Sensor nie eintritt.
        source.Sample(period, scheduler);

    /// <summary>Durchschnitt pro Fenster. Kein Wert geht verloren, leere Fenster liefern nichts.</summary>
    public static IObservable<double> AveragePer(this IObservable<double> source, TimeSpan period, IScheduler scheduler) =>
        // Buffer verwirft nichts, es sammelt. Der Preis: Die Anzeige hinkt ein Fenster hinterher.
        source
            .Buffer(period, scheduler)
            .Where(batch => batch.Count > 0)
            .Select(batch => batch.Average());
}
