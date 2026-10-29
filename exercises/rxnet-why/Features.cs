// Lektion rxnet-why: Wann lohnt sich reaktive Programmierung?
//
// Aufgabe: Schreib drei Features aus deinem Projekt auf, bei denen Werte über die
// Zeit eintreffen, und prüf jedes mit der Checkliste aus dem Handout.
//
// Hier gibt es keinen Rx-Code. Die Tests prüfen nur, dass das Projekt baut,
// System.Reactive läuft und deine Liste vollständig ist.

namespace Exercises.RxNetWhy;

public static class Features
{
    // TODO: Trag drei Features ein, zum Beispiel:
    // new("Produktsuche während der Eingabe", Source.Ui, [Need.Throttle, Need.Cancel],
    //     "Jeder Tastendruck ist ein neuer Wert, veraltete Requests müssen abbrechen."),
    public static IReadOnlyList<StreamFeature> All { get; } = [];
}
