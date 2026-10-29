// Lektion rxnet-time-backpressure: Was tun, wenn mehr Events kommen, als du verarbeitest?
//
// Aufgabe: Zeig aus einem schnellen Sensor-Stream einen gedrosselten Live-Wert mit
// Sample und einen Durchschnitt pro Zeitfenster mit Buffer an.
//
// Ein Observable kennt keinen Rückkanal zum Producer. Du entscheidest, was mit zu
// vielen Werten passiert: verwerfen (Sample) oder sammeln (Buffer).

namespace Exercises.RxNetTimeBackpressure;

public static class Sensor
{
    /// <summary>Ein Sensor, der perSecond Werte pro Sekunde schickt. Der n-te Wert ist read(n).</summary>
    public static IObservable<double> Create(int perSecond, IScheduler scheduler, Func<long, double> read) =>
        Observable
            .Interval(TimeSpan.FromTicks(TimeSpan.TicksPerSecond / perSecond), scheduler)
            .Select(read);

    /// <summary>Live-Wert: am Ende jedes Fensters der neueste Wert, alles dazwischen fällt weg.</summary>
    public static IObservable<double> LiveValue(this IObservable<double> source, TimeSpan period, IScheduler scheduler)
    {
        // TODO: Sample mit period und scheduler
        throw new NotImplementedException("TODO: LiveValue");
    }

    /// <summary>Durchschnitt pro Fenster. Kein Wert geht verloren, leere Fenster liefern nichts.</summary>
    public static IObservable<double> AveragePer(this IObservable<double> source, TimeSpan period, IScheduler scheduler)
    {
        // TODO: Buffer mit period und scheduler, leere Fenster mit Where weglassen, Average
        throw new NotImplementedException("TODO: AveragePer");
    }
}
