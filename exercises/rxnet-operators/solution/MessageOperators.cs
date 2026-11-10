namespace Exercises.RxNetOperators;

public static class MessageOperators
{
    /// <summary>Lets only messages of one type through, without directly repeated Ids, and returns their payload.</summary>
    public static IObservable<TPayload> OfMessageType<TPayload>(this IObservable<Message> source, string type) =>
        source
            .Where(message => message.Type == type)
            // The key selector compares only the Id, not the whole record.
            .DistinctUntilChanged(message => message.Id)
            .Select(message => (TPayload)message.Payload!);

    /// <summary>Counts the messages of one type without duplicates, the running count after every message.</summary>
    public static IObservable<int> CountOfType(this IObservable<Message> source, string type) =>
        // Scan instead of Aggregate: Aggregate only emits after OnCompleted, but a message stream never ends.
        source
            .OfMessageType<object?>(type)
            .Scan(0, (count, _) => count + 1);
}
