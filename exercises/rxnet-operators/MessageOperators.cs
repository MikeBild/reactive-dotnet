// Lektion rxnet-operators: Operatoren sind Extension Methods
//
// Aufgabe: Verarbeite Nachrichten zum Bestellstatus: nach Typ filtern, Duplikate mit
// DistinctUntilChanged weglassen und mit Scan zählen, als eigene Extension Methods.
//
// Ein eigener Operator ist nur eine Funktion von IObservable<A> nach IObservable<B>.

namespace Exercises.RxNetOperators;

public static class MessageOperators
{
    /// <summary>Lässt nur Nachrichten eines Typs durch, ohne direkt wiederholte Ids, und liefert deren Payload.</summary>
    public static IObservable<TPayload> OfMessageType<TPayload>(this IObservable<Message> source, string type)
    {
        // TODO: Where nach Type, DistinctUntilChanged nach Id, Select auf (TPayload)Payload!
        throw new NotImplementedException("TODO: OfMessageType");
    }

    /// <summary>Zählt die Nachrichten eines Typs ohne Duplikate, nach jeder Nachricht der Zwischenstand.</summary>
    public static IObservable<int> CountOfType(this IObservable<Message> source, string type)
    {
        // TODO: OfMessageType wiederverwenden und mit Scan zählen
        throw new NotImplementedException("TODO: CountOfType");
    }
}
