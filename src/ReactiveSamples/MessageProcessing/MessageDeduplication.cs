using System.Reactive.Linq;

namespace ReactiveSamples.MessageProcessing;

/// <summary>
/// Modernisierung von Rx_MessageProcessing (2012):
/// - Tuple&lt;IMyMessage, IMyMessage&gt; + zwei fast identische Klassen → ein record MessagePair.
/// - PropertyComparer per Reflection (mit object-== und zu Equals inkonsistentem GetHashCode)
///   → typsicherer Comparer.
/// Die fachliche Regel bleibt erhalten: ein Paar gilt als "unverändert", wenn sich die Daten der
/// ersten ODER der zweiten Nachricht nicht geändert haben.
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
    /// <summary>Kurzform, wenn ein Schlüssel reicht (RxJS: distinctUntilChanged / distinctUntilKeyChanged).</summary>
    public static IObservable<MessagePair> DistinctByFirst(this IObservable<MessagePair> source) =>
        source.DistinctUntilChanged(pair => pair.First.Data);

    /// <summary>Die Original-Regel ("erste ODER zweite Nachricht gleich") braucht einen Comparer.</summary>
    public static IObservable<MessagePair> DistinctByEitherData(this IObservable<MessagePair> source) =>
        source.DistinctUntilChanged(EitherDataComparer.Instance);

    private sealed class EitherDataComparer : IEqualityComparer<MessagePair>
    {
        public static readonly EitherDataComparer Instance = new();

        public bool Equals(MessagePair? previous, MessagePair? current) =>
            previous is not null && current is not null &&
            (previous.First.Data == current.First.Data || previous.Second.Data == current.Second.Data);

        // DistinctUntilChanged ruft nur Equals auf; eine konstante Hash-Funktion ist für eine
        // nicht-transitive Regel die einzig ehrliche Implementierung.
        public int GetHashCode(MessagePair obj) => 0;
    }
}
