// Lesson rxnet-operators: Operators are Extension Methods
//
// Task: Process order status messages: filter by type, drop duplicates with
// DistinctUntilChanged, and count with Scan, as your own Extension Methods.
//
// A custom Operator is just a function from IObservable<A> to IObservable<B>.

namespace Exercises.RxNetOperators;

public static class MessageOperators
{
    /// <summary>Lets only messages of one type through, without directly repeated Ids, and returns their payload.</summary>
    public static IObservable<TPayload> OfMessageType<TPayload>(this IObservable<Message> source, string type)
    {
        // TODO: Where by Type, DistinctUntilChanged by Id, Select to (TPayload)Payload!
        throw new NotImplementedException("TODO: OfMessageType");
    }

    /// <summary>Counts the messages of one type without duplicates, the running count after every message.</summary>
    public static IObservable<int> CountOfType(this IObservable<Message> source, string type)
    {
        // TODO: Reuse OfMessageType and count with Scan
        throw new NotImplementedException("TODO: CountOfType");
    }
}
