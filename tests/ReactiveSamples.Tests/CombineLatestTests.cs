using Microsoft.Reactive.Testing;
using ReactiveSamples.CombineLatest;

namespace ReactiveSamples.Tests;

/// <summary>
/// Das Original befeuerte zwei Subjects von Hand. Im Test ersetzt CreateHotObservable die Subjects:
/// gleiche Abfolge, aber deklarativ und mit Zeitstempeln (RxJS: hot('-a--b-') Marble-Syntax).
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
