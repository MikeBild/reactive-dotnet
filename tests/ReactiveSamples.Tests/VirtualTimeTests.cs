using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Microsoft.Reactive.Testing;

namespace ReactiveSamples.Tests;

/// <summary>
/// Modernization of gist "SimpleRxTesting" (2012): test time-dependent streams without mocks and without waiting.
/// The TestScheduler replaces the clock (RxJS: TestScheduler / marble tests).
/// </summary>
public class VirtualTimeTests : ReactiveTest
{
    [Fact]
    public void Interval_with_TestScheduler_like_2012()
    {
        var scheduler = new TestScheduler();
        var actual = new List<long>();

        Observable.Interval(TimeSpan.FromSeconds(1), scheduler).Take(5).Subscribe(actual.Add);
        scheduler.Start(); // runs until no more work is scheduled – in microseconds instead of 5 seconds

        Assert.Equal([0L, 1, 2, 3, 4], actual);
    }

    [Fact]
    public void Interval_with_recorded_notifications_and_exact_times()
    {
        var scheduler = new TestScheduler();
        var second = TimeSpan.FromSeconds(1).Ticks;
        const long subscribed = 200;

        // Careful: Start(create) without times subscribes at tick 200 and disposes at tick 1000 –
        // for intervals in the seconds range, NOTHING would arrive. Hence an explicit end.
        // (Avoid tick 0: the TestScheduler schedules work "now" at tick+1.)
        var observer = scheduler.Start(
            () => Observable.Interval(TimeSpan.FromSeconds(1), scheduler).Take(3),
            created: 100, subscribed: subscribed, disposed: 10 * second);

        Assert.Equal(
            [
                OnNext(subscribed + 1 * second, 0L),
                OnNext(subscribed + 2 * second, 1L),
                OnNext(subscribed + 3 * second, 2L),
                OnCompleted<long>(subscribed + 3 * second),
            ],
            observer.Messages);
    }

    [Fact]
    public void Default_Start_disposes_at_tick_1000_pitfall()
    {
        var scheduler = new TestScheduler();
        var observer = scheduler.Start(() => Observable.Interval(TimeSpan.FromSeconds(1), scheduler));
        Assert.Empty(observer.Messages);
    }

    [Fact]
    public async Task Async_interop_ToTask_and_FirstAsync_in_virtual_time()
    {
        var scheduler = new TestScheduler();
        var answer = Observable.Return(42).Delay(TimeSpan.FromMinutes(5), scheduler);

        var task = answer.FirstAsync().ToTask(); // RxJS: firstValueFrom(answer)
        Assert.False(task.IsCompleted);

        scheduler.AdvanceBy(TimeSpan.FromMinutes(5).Ticks);
        Assert.Equal(42, await task);
    }

    [Fact]
    public async Task Observables_are_awaitable_and_yield_the_last_value()
    {
        // await on an IObservable returns the LAST value (RxJS: lastValueFrom).
        Assert.Equal(3, await Observable.Range(1, 3));
        Assert.Equal(new[] { 1, 2, 3 }, await Observable.Range(1, 3).ToArray());
    }
}
