using System.Reactive.Linq;

namespace ReactiveSamples.MessageProcessing;

/// <summary>
/// Modernization of Rx_MessageProcessing (2012):
/// - Tuple&lt;IMyMessage, IMyMessage&gt; + two almost identical classes → one record MessagePair.
/// - PropertyComparer via reflection (with object == and a GetHashCode inconsistent with Equals)
///   → type-safe comparer.
/// The business rule stays the same: a pair counts as "unchanged" if the data of the
/// first OR the second message has not changed.
/// </summary>
public interface IMessage
{
    string Data { get; }
}

public sealed record OrderMessage(string Data, string? Id = null) : IMessage;
public sealed record InvoiceMessage(string Data, string? Id = null) : IMessage;

public sealed record MessagePair(IMessage First, IMessage Second)
{
    public override string ToString() => $"{First.Data}:{Second.Data}";
}

public static class MessageDeduplication
{
    /// <summary>Short form when one key is enough (RxJS: distinctUntilChanged / distinctUntilKeyChanged).</summary>
    public static IObservable<MessagePair> DistinctByFirst(this IObservable<MessagePair> source) =>
        source.DistinctUntilChanged(pair => pair.First.Data);

    /// <summary>The original rule ("first OR second message equal") needs a comparer.</summary>
    public static IObservable<MessagePair> DistinctByEitherData(this IObservable<MessagePair> source) =>
        source.DistinctUntilChanged(EitherDataComparer.Instance);

    private sealed class EitherDataComparer : IEqualityComparer<MessagePair>
    {
        public static readonly EitherDataComparer Instance = new();

        public bool Equals(MessagePair? previous, MessagePair? current) =>
            previous is not null && current is not null &&
            (previous.First.Data == current.First.Data || previous.Second.Data == current.Second.Data);

        // DistinctUntilChanged only calls Equals; a constant hash function is the only honest
        // implementation for a non-transitive rule.
        public int GetHashCode(MessagePair obj) => 0;
    }
}
