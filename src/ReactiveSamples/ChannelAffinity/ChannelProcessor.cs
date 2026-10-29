using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace ReactiveSamples.ChannelAffinity;

/// <summary>
/// Modernisierung von Gist "last one close the door" (RxScheduling, 2013):
/// Kommandos werden pro Kanal gruppiert (GroupBy); jeder Kanal läuft auf einem eigenen
/// EventLoopScheduler (= ein dedizierter Thread), d. h. pro Kanal sequenziell, kanalübergreifend parallel.
///
/// Probleme des Originals und was hier anders ist:
/// - Busy-Wait auf einem nicht threadsicheren Zähler + Environment.Exit in Dispose
///   → das Ende wird über OnCompleted signalisiert und mit await (ToTask) abgewartet, inkl. Timeout.
/// - Pro Gruppe ein neuer EventLoopScheduler, der nie entsorgt wurde (Thread-Leck)
///   → Scheduler wird beim Abschluss der Gruppe disposed.
/// - Mutable Klassen Command/Channel mit handgeschriebenem Equals → records.
/// </summary>
public sealed record Channel(int Id)
{
    public override string ToString() => $"Channel-{Id}";
}

public sealed record Command(Channel Channel, int Sequence, TimeSpan Work);

public sealed record Processed(Command Command, int ThreadId);

public static class ChannelProcessor
{
    /// <summary>
    /// Verarbeitet alle Kommandos; das Ergebnis-Observable endet, wenn die Quelle endet
    /// und jeder Kanal seine Warteschlange abgearbeitet hat ("drain").
    /// </summary>
    public static IObservable<Processed> Process(IObservable<Command> commands, Action<Command> handler) =>
        commands
            .GroupBy(command => command.Channel)
            .SelectMany(channel => Observable.Using(
                () => new EventLoopScheduler(start => new Thread(start) { IsBackground = true, Name = channel.Key.ToString() }),
                scheduler => channel
                    .ObserveOn(scheduler)
                    .Select(command =>
                    {
                        handler(command);
                        return new Processed(command, Environment.CurrentManagedThreadId);
                    })));

    /// <summary>Graceful Shutdown: auf das Ende warten, aber höchstens <paramref name="timeout"/>.</summary>
    public static async Task<IReadOnlyList<Processed>> DrainAsync(
        IObservable<Command> commands, Action<Command> handler, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var result = await Process(commands, handler)
            .ToList()
            .Timeout(timeout)
            .ToTask(cancellationToken);
        return result.AsReadOnly();
    }

    /// <summary>Hilfsfunktion für Tests/Demo: welcher Kanal lief auf welchen Threads?</summary>
    public static IReadOnlyDictionary<Channel, IReadOnlySet<int>> ThreadsPerChannel(IEnumerable<Processed> processed) =>
        processed
            .GroupBy(p => p.Command.Channel)
            .ToDictionary(g => g.Key, g => (IReadOnlySet<int>)g.Select(p => p.ThreadId).ToHashSet());
}

/// <summary>Kleiner Helfer, um gleichzeitige Ausführung pro Kanal zu erkennen (darf nie passieren).</summary>
public sealed class ConcurrencyProbe
{
    private readonly ConcurrentDictionary<Channel, int> _active = new();
    private int _violations;

    public int Violations => _violations;

    public void Run(Command command)
    {
        if (_active.AddOrUpdate(command.Channel, 1, (_, n) => n + 1) > 1)
        {
            Interlocked.Increment(ref _violations);
        }
        Thread.Sleep(command.Work);
        _active.AddOrUpdate(command.Channel, 0, (_, n) => n - 1);
    }
}
