using Microsoft.Reactive.Testing;
using ReactiveSamples.CombineLatest;

namespace ReactiveSamples.Tests;

/// <summary>
/// The original fed two Subjects by hand. In the test, CreateHotObservable replaces the Subjects:
/// same sequence, but declarative and with timestamps (RxJS: hot('-a--b-') marble syntax).
/// </summary>
public class CombineLatestTests : ReactiveTest
{
    [Fact]
    public void Emits_latest_pair_once_both_sides_have_a_value()
    {
        var scheduler = new TestScheduler();
        var numbers = scheduler.CreateHotObservable(OnNext(300, 42), OnNext(500, 99));
        var texts = scheduler.CreateHotObservable(OnNext(400, "hello"), OnNext(600, "world"));

        var observer = scheduler.Start(() => LatestPairs.Combine(numbers, texts));

        Assert.Equal(
            [
                OnNext(400, (42, "hello")),
                OnNext(500, (99, "hello")),
                OnNext(600, (99, "world")),
            ],
            observer.Messages);
    }

    [Fact]
    public void Formats_like_the_original() => Assert.Equal("42,hello", LatestPairs.Format((42, "hello")));
}
