using System.Reactive.Linq;
using ReactiveSamples.Coins;

namespace ReactiveSamples.Tests;

public class CoinChangerTests
{
    private static readonly CoinCount[] NinetyNine = [new(50, 1), new(20, 2), new(5, 1), new(2, 2)];

    [Fact]
    public void Pull_with_LINQ_Aggregate() => Assert.Equal(NinetyNine, CoinChanger.Change(99));

    [Fact]
    public async Task Push_with_Rx_Aggregate_gives_the_same_result() =>
        Assert.Equal(NinetyNine, await CoinChanger.ChangeAsObservable(99));

    [Fact]
    public async Task Scan_shows_every_intermediate_state()
    {
        var remainders = await CoinChanger
            .ChangeSteps(99, new[] { 100, 50, 20, 10, 5, 2, 1 }.ToObservable())
            .Select(s => s.Remainder)
            .ToArray();
        Assert.Equal([99, 49, 9, 9, 4, 0, 0], remainders);
    }

    [Fact]
    public void Pure_function_can_be_called_twice()
    {
        // Mit "amount %= n" in der Query lieferte eine zweite Enumeration ein anderes Ergebnis.
        Assert.Equal(CoinChanger.Change(187), CoinChanger.Change(187));
        Assert.Empty(CoinChanger.Change(0));
    }
}
