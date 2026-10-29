// Beispiellösung für rxnet-why. Deine Features sehen anders aus.
// Wichtig ist nur, dass du für jedes begründen kannst, warum es ein Stream ist.

namespace Exercises.RxNetWhy;

public static class Features
{
    public static IReadOnlyList<StreamFeature> All { get; } =
    [
        new("Produktsuche während der Eingabe", Source.Ui, [Need.Throttle, Need.Cancel],
            "Jeder Tastendruck ist ein neuer Wert, veraltete Requests müssen abbrechen."),
        new("Live-Status einer Bestellung", Source.WebSocket, [Need.Combine, Need.Retry],
            "Der Server meldet jeden Statuswechsel, nach einem Verbindungsabbruch soll es neu verbinden."),
        new("Status eines Exports abfragen", Source.Polling, [Need.Cancel, Need.Retry],
            "Bis der Export fertig ist, kommen viele Antworten, beim Schließen der Ansicht endet das Polling."),
    ];
}
