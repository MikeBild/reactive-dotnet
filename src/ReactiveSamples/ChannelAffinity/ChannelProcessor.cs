using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace ReactiveSamples.ChannelAffinity;

/// <summary>
/// Modernization of gist "last one close the door" (RxScheduling, 2013):
/// Commands are grouped per channel (GroupBy); each channel runs on its own
/// EventLoopScheduler (= a dedicated thread), i.e. sequential per channel, parallel across channels.
///
/// Problems of the original and what is different here:
/// - Busy wait on a non-thread-safe counter + Environment.Exit in Dispose
///   → the end is signaled through OnCompleted and awaited with await (ToTask), including a timeout.
/// - A new EventLoopScheduler per group that was never disposed (thread leak)
///   → the scheduler is disposed when the group completes.
/// - Mutable classes Command/Channel with hand-written Equals → records.
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
    /// Processes all commands; the result Observable ends when the source ends
    /// and each channel has worked through its queue ("drain").
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

    /// <summary>Graceful shutdown: wait for the end, but at most <paramref name="timeout"/>.</summary>
    public static async Task<IReadOnlyList<Processed>> DrainAsync(
        IObservable<Command> commands, Action<Command> handler, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var result = await Process(commands, handler)
            .ToList()
            .Timeout(timeout)
            .ToTask(cancellationToken);
        return result.AsReadOnly();
    }

    /// <summary>Helper for tests/demo: which channel ran on which threads?</summary>
    public static IReadOnlyDictionary<Channel, IReadOnlySet<int>> ThreadsPerChannel(IEnumerable<Processed> processed) =>
        processed
            .GroupBy(p => p.Command.Channel)
            .ToDictionary(g => g.Key, g => (IReadOnlySet<int>)g.Select(p => p.ThreadId).ToHashSet());
}

/// <summary>Small helper to detect concurrent execution per channel (must never happen).</summary>
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
