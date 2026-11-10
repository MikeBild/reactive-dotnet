using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveSamples.MessageBus;

namespace ReactiveSamples.Saga;

/// <summary>
/// Modernization of gist "ZeroMqDocumentSaga" (Minimod.MessageProcessor + ZeroMQ, 2011):
/// A document travels as a message from processor to processor; each one fills in its chapter,
/// moves the bookmark on, and sends the document back onto the bus (Routing Slip).
///
/// What is different:
/// - ZeroMQ sockets and the Minimod packages are replaced by the in-memory <see cref="RxMessageBus"/>.
///   In production, a broker (RabbitMQ, NATS, Azure Service Bus, Kafka) handles this today
///   with a saga framework (MassTransit, NServiceBus, Wolverine) – see README.
/// - The message used to be mutated and resent through a hidden context.
///   Today: immutable record + <c>with</c> expression, Bookmark as an enum instead of a string.
/// </summary>
public enum Bookmark { Chapter1, Chapter2, Done }

public sealed record ChaptersDocument(
    Guid DocumentId,
    Bookmark Bookmark,
    string Chapter1,
    string Chapter2,
    string ThankYouMessage) : ICommandMessage
{
    public static ChaptersDocument New(string thankYou) =>
        new(Guid.NewGuid(), Bookmark.Chapter1, "Empty", "Empty", thankYou);
}

public static class DocumentSaga
{
    /// <summary>One processor = filter on a bookmark + pure transformation + forwarding.</summary>
    private static IDisposable Step(RxMessageBus bus, Bookmark at, Func<ChaptersDocument, ChaptersDocument> fill) =>
        bus.Messages<ChaptersDocument>()
            .Where(document => document.Bookmark == at)
            .Select(fill)
            .Subscribe(bus.Send);

    /// <summary>Wires up all processors and returns the stream of finished documents.</summary>
    public static (IObservable<ChaptersDocument> Completed, IDisposable Wiring) Wire(RxMessageBus bus)
    {
        var chapter1 = Step(bus, Bookmark.Chapter1, d => d with { Chapter1 = "Filled", Bookmark = Bookmark.Chapter2 });
        var chapter2 = Step(bus, Bookmark.Chapter2, d => d with { Chapter2 = "Filled", Bookmark = Bookmark.Done });
        var completed = bus.Messages<ChaptersDocument>().Where(d => d.Bookmark == Bookmark.Done);
        return (completed, new CompositeDisposable(chapter1, chapter2));
    }
}
