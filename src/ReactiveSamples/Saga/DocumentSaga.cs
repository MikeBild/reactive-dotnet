using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveSamples.MessageBus;

namespace ReactiveSamples.Saga;

/// <summary>
/// Modernisierung von Gist "ZeroMqDocumentSaga" (Minimod.MessageProcessor + ZeroMQ, 2011):
/// Ein Dokument wandert als Nachricht von Prozessor zu Prozessor; jeder füllt sein Kapitel,
/// setzt das Lesezeichen weiter und schickt das Dokument zurück auf den Bus (Routing Slip).
///
/// Was anders ist:
/// - ZeroMQ-Sockets und die Minimod-Pakete sind ersetzt durch den In-Memory-<see cref="RxMessageBus"/>.
///   In Produktion übernimmt das heute ein Broker (RabbitMQ, NATS, Azure Service Bus, Kafka)
///   mit einem Saga-Framework (MassTransit, NServiceBus, Wolverine) – siehe README.
/// - Die Nachricht wurde früher mutiert und über einen versteckten Kontext erneut gesendet.
///   Heute: unveränderlicher record + <c>with</c>-Ausdruck, Bookmark als enum statt String.
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
    /// <summary>Ein Prozessor = Filter auf ein Lesezeichen + reine Transformation + Weitersenden.</summary>
    private static IDisposable Step(RxMessageBus bus, Bookmark at, Func<ChaptersDocument, ChaptersDocument> fill) =>
        bus.Messages<ChaptersDocument>()
            .Where(document => document.Bookmark == at)
            .Select(fill)
            .Subscribe(bus.Send);

    /// <summary>Verdrahtet alle Prozessoren und liefert den Strom fertiger Dokumente.</summary>
    public static (IObservable<ChaptersDocument> Completed, IDisposable Wiring) Wire(RxMessageBus bus)
    {
        var chapter1 = Step(bus, Bookmark.Chapter1, d => d with { Chapter1 = "Filled", Bookmark = Bookmark.Chapter2 });
        var chapter2 = Step(bus, Bookmark.Chapter2, d => d with { Chapter2 = "Filled", Bookmark = Bookmark.Done });
        var completed = bus.Messages<ChaptersDocument>().Where(d => d.Bookmark == Bookmark.Done);
        return (completed, new CompositeDisposable(chapter1, chapter2));
    }
}
