using System.Reactive.Linq;
using ReactiveSamples.ChannelAffinity;

namespace ReactiveSamples.Tests;

/// <summary>Echte Threads (EventLoopScheduler) – hier wird bewusst nicht virtualisiert.</summary>
public class ChannelAffinityTests
{
    private static IObservable<Command> Commands(int channels, int perChannel, TimeSpan work) =>
        Enumerable.Range(0, channels * perChannel)
            .Select(i => new Command(new Channel(i % channels), i / channels, work))
            .ToObservable();

    [Fact]
    public async Task Each_channel_runs_sequentially_on_its_own_thread_and_everything_drains()
    {
        var probe = new ConcurrencyProbe();

        var processed = await ChannelProcessor.DrainAsync(
            Commands(channels: 3, perChannel: 5, work: TimeSpan.FromMilliseconds(5)),
            probe.Run,
            timeout: TimeSpan.FromSeconds(10));

        Assert.Equal(15, processed.Count);
        Assert.Equal(0, probe.Violations);

        var threads = ChannelProcessor.ThreadsPerChannel(processed);
        Assert.Equal(3, threads.Count);
        Assert.All(threads.Values, set => Assert.Single(set));

        foreach (var channel in processed.GroupBy(p => p.Command.Channel))
        {
            Assert.Equal([0, 1, 2, 3, 4], channel.Select(p => p.Command.Sequence));
        }
    }

    [Fact]
    public async Task Drain_gives_up_after_the_timeout_instead_of_Environment_Exit()
    {
        await Assert.ThrowsAsync<TimeoutException>(() => ChannelProcessor.DrainAsync(
            Commands(channels: 1, perChannel: 3, work: TimeSpan.FromSeconds(1)),
            command => Thread.Sleep(command.Work),
            timeout: TimeSpan.FromMilliseconds(100)));
    }
}
