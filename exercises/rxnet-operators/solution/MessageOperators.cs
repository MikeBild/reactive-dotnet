namespace Exercises.RxNetOperators;

public static class MessageOperators
{
    /// <summary>Lässt nur Nachrichten eines Typs durch, ohne direkt wiederholte Ids, und liefert deren Payload.</summary>
    public static IObservable<TPayload> OfMessageType<TPayload>(this IObservable<Message> source, string type) =>
        source
            .Where(message => message.Type == type)
            // Der Key-Selector vergleicht nur die Id, nicht den ganzen Record.
            .DistinctUntilChanged(message => message.Id)
            .Select(message => (TPayload)message.Payload!);

    /// <summary>Zählt die Nachrichten eines Typs ohne Duplikate, nach jeder Nachricht der Zwischenstand.</summary>
    public static IObservable<int> CountOfType(this IObservable<Message> source, string type) =>
        // Scan statt Aggregate: Aggregate liefert erst nach OnCompleted, ein Nachrichtenstrom endet aber nie.
        source
            .OfMessageType<object?>(type)
            .Scan(0, (count, _) => count + 1);
}
